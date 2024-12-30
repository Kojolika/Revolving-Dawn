using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Serialization;
using Tooling.Logging;
using Tooling.StaticData.Validation;
using UnityEditor;
using UnityEngine;

namespace Tooling.StaticData
{
    public class StaticDatabase
    {
        private static StaticDatabase instance;

        public static StaticDatabase Instance
        {
            get
            {
                instance ??= new StaticDatabase();
                return instance;
            }
        }

        private static readonly string StaticDataDirectory = Path.Join(Application.dataPath, "StaticData");

        private readonly Dictionary<Type, Dictionary<string, StaticData>> staticDataDictionary = new();

        public Dictionary<Type, Dictionary<StaticData, List<string>>> validationErrors { get; private set; } = new();

        /// <summary>
        /// Notifies when validation has just been completed. The <see cref="validationErrors"/> will be populated after
        /// this is called.
        /// </summary>
        public event Action OnValidationCompleted;

        private readonly JsonSerializer jsonSerializer = new()
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CustomContractResolver(),
            Converters =
            {
                new AssetReferenceConverter(),
                new ColorConverter(),
                new StaticDataConverter()
            },
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.Auto
        };

        private readonly List<StaticDataReferenceHandle> queuedInjections = new();

        private StaticDatabase()
        {
        }

        public void BuildDictionaryFromJson()
        {
            var staticDataTypes = typeof(StaticData).Assembly.GetTypes()
                .Where(type => typeof(StaticData).IsAssignableFrom(type) && !type.IsAbstract)
                .ToList();

            Clear();

            foreach (var type in staticDataTypes)
            {
                var instanceDictionary = new Dictionary<string, StaticData>();
                var typeDirectory = Path.GetFullPath(Path.Join(StaticDataDirectory, type.Name));

                if (Directory.Exists(typeDirectory))
                {
                    foreach (var file in Directory.EnumerateFiles(typeDirectory, "*.json"))
                    {
                        using var streamReader = File.OpenText(file);

                        StaticData staticDataFromJson = null;
                        try
                        {
                            staticDataFromJson = (StaticData)jsonSerializer.Deserialize(streamReader, type);
                        }
                        catch (Exception e)
                        {
                            MyLogger.LogError(e.Message);
                        }

                        if (staticDataFromJson == null)
                        {
                            MyLogger.LogError($"Static Data of type {type.Name} could not be deserialized.");
                            continue;
                        }

                        var fileNameWithExtension = new FileInfo(file).Name;
                        staticDataFromJson.Name = fileNameWithExtension[..^".json".Length];

                        instanceDictionary.Add(staticDataFromJson.Name, staticDataFromJson);
                    }
                }

                staticDataDictionary.Add(type, instanceDictionary);
            }

            InjectReferences();
        }

        /// <summary>
        /// Queues a <see cref="StaticData"/> to have its references to other <see cref="StaticData"/> injected
        /// after every <see cref="StaticData"/> is deserialized.
        /// </summary>
        public void QueueReferenceForInject(Type staticDataType, string instanceName, StaticData staticData, string propertyName)
        {
            queuedInjections.Add(new StaticDataReferenceHandle(staticDataType, instanceName, staticData, propertyName));
        }

        /// <summary>
        /// Queues a <see cref="StaticData"/> to have its references to other <see cref="StaticData"/> injected
        /// after every <see cref="StaticData"/> is deserialized.
        /// <remarks>The other <see cref="StaticData"/> references are part of an array like field.</remarks>
        /// </summary>
        public void QueueReferenceInArrayForInject(
            Type staticDataType,
            string instanceName,
            StaticData staticData,
            string propertyName,
            int index)
        {
            queuedInjections.Add(
                new StaticDataReferenceHandle(
                    staticDataType,
                    instanceName,
                    staticData,
                    propertyName,
                    index
                )
            );
        }

        /// <summary>
        /// Injects all <see cref="StaticData"/> references once deserialization completes.
        /// This is so we can serialize references in multiple files instead of copying the <see cref="StaticData"/>
        /// data everywhere.
        /// </summary>
        private void InjectReferences()
        {
            foreach (var referenceHandle in queuedInjections)
            {
                var staticDataType = referenceHandle.ObjectWithReference.GetType();
                var staticDataField = staticDataType.GetField(referenceHandle.PropertyName,
                    EditorWindow.BindingFlagsToSelectStaticDataFields);

                if (staticDataField == null)
                {
                    MyLogger.LogError($"Could not find field {referenceHandle.PropertyName} " +
                                      $"on Static Data of type {staticDataType}");

                    continue;
                }

                if (referenceHandle.ArrayIndex < 0)
                {
                    staticDataField.SetValue(
                        referenceHandle.ObjectWithReference,
                        GetStaticDataInstance(referenceHandle.Type, referenceHandle.InstanceName)
                    );
                }
                else
                {
                    var list = (IList)(staticDataField.GetValue(referenceHandle.ObjectWithReference)
                                       ?? Activator.CreateInstance(staticDataField.FieldType));
                    list[referenceHandle.ArrayIndex] = GetStaticDataInstance(referenceHandle.Type, referenceHandle.InstanceName);

                    staticDataField.SetValue(referenceHandle.ObjectWithReference, list);
                }
            }
        }

        private struct StaticDataReferenceHandle
        {
            /// <summary>
            /// The type of static data being referenced.
            /// </summary>
            public readonly Type Type;

            /// <summary>
            /// The instance name of the <see cref="Type"/> being referenced.
            /// </summary>
            public readonly string InstanceName;

            /// <summary>
            /// The StaticData that has the reference to another StaticData.
            /// </summary>
            public readonly StaticData ObjectWithReference;

            /// <summary>
            /// The propertyName that holds the reference to another StaticData.
            /// </summary>
            public readonly string PropertyName;

            /// <summary>
            /// If the property is an array, this will be set to a value > -1.
            /// </summary>
            public readonly int ArrayIndex;

            public StaticDataReferenceHandle(Type type,
                string instanceName,
                StaticData objectWithReference,
                string propertyName,
                int arrayIndex = -1)
            {
                Type = type;
                InstanceName = instanceName;
                ObjectWithReference = objectWithReference;
                PropertyName = propertyName;
                ArrayIndex = arrayIndex;
            }
        }

        public void ValidateStaticData()
        {
            validationErrors = Validator.ValidateObjects(
                GetAllStaticDataInstances(),
                EditorWindow.BindingFlagsToSelectStaticDataFields
            );

            OnValidationCompleted?.Invoke();
        }

        public void UpdateInstancesForType(Type type, List<StaticData> instances)
        {
            if (!staticDataDictionary.TryGetValue(type, out var instanceDict))
            {
                return;
            }

            instanceDict.Clear();
            foreach (var inst in instances)
            {
                instanceDict[inst.Name] = inst;
            }

            staticDataDictionary[type] = instanceDict;
        }

        public StaticData GetStaticDataInstance(Type type, string instanceName)
        {
            if (staticDataDictionary.TryGetValue(type, out var instanceDictionary)
                && instanceDictionary.TryGetValue(instanceName, out var dataInstance))
            {
                return dataInstance;
            }

            return null;
        }

        private List<StaticData> GetAllStaticDataInstances()
        {
            return staticDataDictionary.SelectMany(kvp => kvp.Value.Values).ToList();
        }

        public List<StaticData> GetInstancesForType(Type type)
        {
            if (staticDataDictionary.TryGetValue(type, out var instanceDictionary))
            {
                return instanceDictionary.Values.ToList();
            }

            return null;
        }

        public List<Type> GetAllStaticDataTypes()
        {
            return staticDataDictionary.Select(kvp => kvp.Key).ToList();
        }

        public void Remove(Type type, string instanceName)
        {
            if (staticDataDictionary.TryGetValue(type, out var instanceDictionary))
            {
                instanceDictionary.Remove(instanceName);
            }
        }

        public void Add(StaticData staticData)
        {
            if (staticDataDictionary.TryGetValue(staticData.GetType(), out var instanceDictionary))
            {
                instanceDictionary[staticData.Name] = staticData;
            }
        }

        public async UniTask SaveAllStaticDataToJson()
        {
            var writeTasks = new List<UniTask>();
            foreach (var kvp in staticDataDictionary)
            {
                var staticDataFilePath = Path.GetFullPath(Path.Join(StaticDataDirectory, kvp.Key.Name));
                var directoryInfo = Directory.CreateDirectory(staticDataFilePath);

                foreach (var directory in directoryInfo.GetDirectories())
                {
                    directory.Delete();
                }

                foreach (var staticDataInstance in kvp.Value.Values)
                {
                    writeTasks.Add(SaveInstanceToJson(staticDataInstance, staticDataFilePath));
                }
            }

            EditorUtility.DisplayProgressBar("Saving to Json", "Saving...", 0.5f);

            try
            {
                await UniTask.WhenAll(writeTasks);
            }
            catch (Exception e)
            {
                MyLogger.LogError(e.Message);
            }

            EditorUtility.ClearProgressBar();

            return;

            // TODO: this doesn't update name changes (maybe just case insensitive)
            // but if you chnage a name from bash.json -> Bash.json it doesn't save as Bash.json
            // Local function
            async UniTask SaveInstanceToJson(StaticData instance, string typeDirectory)
            {
                if (!Directory.Exists(typeDirectory))
                {
                    MyLogger.LogError($"Type directory {typeDirectory} does not exist!");
                    return;
                }

                var filePath = $"{Path.Join(typeDirectory, instance.Name)}.json";
                MyLogger.Log($"Writing {instance.Name} to file: {filePath}");
                await using StreamWriter file = new StreamWriter(filePath);
                using JsonWriter writer = new JsonTextWriter(file);

                jsonSerializer.Serialize(writer, instance);
            }
        }

        public void Clear()
        {
            queuedInjections.Clear();
            staticDataDictionary.Clear();
        }
    }
}