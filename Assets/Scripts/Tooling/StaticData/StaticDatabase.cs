using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Serialization;
using Tooling.Logging;
using Tooling.StaticData.EditorUI.Validation;
using UnityEditor;
using UnityEngine;
using Utils.Extensions;

namespace Tooling.StaticData.EditorUI
{
    [InitializeOnLoad]
    public class StaticDatabase
    {
        static StaticDatabase()
        {
            Instance.BuildDictionaryFromJson();
        }

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

        public event Action       OnStaticDataInstancesBuilt;
        public event Action<Type> InstancesUpdated;

        public Dictionary<Type, Dictionary<StaticData, List<string>>> validationErrors { get; private set; } = new();

        /// <summary>
        /// Notifies when validation has just been completed. The <see cref="validationErrors"/> will be populated after
        /// this is called.
        /// </summary>
        public event Action ValidationCompleted;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            Formatting       = Formatting.Indented,
            ContractResolver = new CustomContractResolver(),
            Converters =
            {
                new AssetReferenceConverter(),
                new ColorConverter(),
                new StaticDataConverter(),
            },
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling      = TypeNameHandling.Auto,
            //TraceWriter = new MyLogger(),
        };

        private static readonly JsonSerializer JsonSerializer = JsonSerializer.Create(JsonSerializerSettings);

        private readonly List<StaticDataReferenceHandle> queuedInjections = new();

        private readonly MainValidator validator = new(
            new List<IValidator>
            {
                new AssetReferenceValidator()
            }
        );

        /// <summary>
        /// Prevents creating this instance externally
        /// </summary>
        private StaticDatabase()
        {
        }

        public void BuildDictionaryFromJson()
        {
            Clear();

            var staticDataTypes = typeof(StaticData).Assembly.GetTypes()
                                                    .Where(type => typeof(StaticData).IsAssignableFrom(type) && !type.IsAbstract)
                                                    .ToList();

            foreach (var type in staticDataTypes)
            {
                var instanceDictionary = new Dictionary<string, StaticData>();
                var typeDirectory      = Path.GetFullPath(Path.Join(StaticDataDirectory, type.Name));

                if (Directory.Exists(typeDirectory))
                {
                    foreach (var file in Directory.EnumerateFiles(typeDirectory, "*.json"))
                    {
                        using var streamReader = File.OpenText(file);

                        StaticData staticDataFromJson = null;

                        try
                        {
                            staticDataFromJson = (StaticData)JsonSerializer.Deserialize(streamReader, type);
                        }
                        catch (Exception e)
                        {
                            MyLogger.LogError($"Exception while deserializing: {e}");
                            MyLogger.LogError($"Static Data of type {type.Name} could not be deserialized. Setting value to default...");
                            staticDataFromJson = Activator.CreateInstance(type) as StaticData;
                        }

                        if (staticDataFromJson == null)
                        {
                            MyLogger.LogError("Static data is still null, is the type wrong...?");
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
        /// Queues an object to have its references to <see cref="StaticData"/> injected
        /// after every <see cref="StaticData"/> is deserialized.
        /// </summary>
        public void QueueReferenceForInject(
            Type   staticDataType,
            string instanceName,
            object obj,
            string propertyName,
            int    arrayIndex = -1)
        {
            queuedInjections.Add(new StaticDataReferenceHandle(staticDataType, instanceName, obj, propertyName, arrayIndex));
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
                var staticDataType  = referenceHandle.ObjectWithReference.GetType();
                var staticDataField = Utils.GetField(staticDataType, referenceHandle.PropertyName);

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
            /// The object that has the reference to a StaticData.
            /// </summary>
            public readonly object ObjectWithReference;

            /// <summary>
            /// The propertyName that holds the reference to another StaticData.
            /// </summary>
            public readonly string PropertyName;

            /// <summary>
            /// If the property is an array, this will be set to a value > -1.
            /// </summary>
            public readonly int ArrayIndex;

            public StaticDataReferenceHandle(
                Type   type,
                string instanceName,
                object objectWithReference,
                string propertyName,
                int    arrayIndex = -1)
            {
                Type                = type;
                InstanceName        = instanceName;
                ObjectWithReference = objectWithReference;
                PropertyName        = propertyName;
                ArrayIndex          = arrayIndex;
            }
        }

        public void ValidateStaticData()
        {
            validationErrors = validator.ValidateObjects(GetAllStaticDataInstances());
            ValidationCompleted?.Invoke();
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
                if (inst == null)
                {
                    return;
                }

                instanceDict[inst.Name] = inst;
            }

            staticDataDictionary[type] = instanceDict;
            InstancesUpdated?.Invoke(type);
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

        public T GetStaticDataInstance<T>(string instanceName) where T : StaticData
        {
            if (staticDataDictionary.TryGetValue(typeof(T), out var instanceDictionary)
             && instanceDictionary.TryGetValue(instanceName, out var dataInstance))
            {
                return dataInstance as T;
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

            return new List<StaticData>();
        }

        public List<T> GetInstancesForType<T>() where T : StaticData
        {
            if (staticDataDictionary.TryGetValue(typeof(T), out var instanceDictionary))
            {
                return instanceDictionary.Values.Select(data => data as T).ToList();
            }

            return new List<T>();
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
                InstancesUpdated?.Invoke(type);
            }
        }

        public void AddOrUpdate(StaticData staticData)
        {
            Type staticDataType = staticData?.GetType();
            if (staticDataType == null)
            {
                return;
            }

            if (staticDataDictionary.TryGetValue(staticDataType, out var instanceDictionary))
            {
                instanceDictionary[staticData.Name] = staticData;
                InstancesUpdated?.Invoke(staticDataType);
            }
        }

        public async UniTask SaveAllStaticDataToJson()
        {
            var writeTasks = new List<UniTask>();
            foreach (var kvp in staticDataDictionary)
            {
                if (kvp.Value.IsNullOrEmpty())
                {
                    continue;
                }

                var staticDataFilePath = Path.GetFullPath(Path.Join(StaticDataDirectory, kvp.Key.Name));
                var directoryInfo      = Directory.CreateDirectory(staticDataFilePath);

                foreach (var directory in directoryInfo.GetDirectories())
                {
                    directory.Delete();
                }

                foreach (var file in directoryInfo.GetFiles())
                {
                    file.Delete();
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
                MyLogger.LogError($"Error saving to json: {e}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            BuildDictionaryFromJson();

            return;

            // TODO: this doesn't update name changes (maybe just case insensitive)
            // but if you chnage a name from bash.json -> Bash.json it doesn't save as Bash.json
            // Local function
            async UniTask SaveInstanceToJson(StaticData instance, string typeDirectory)
            {
                if (instance == null)
                {
                    MyLogger.LogError($"Trying to serialize null StaticData in typeDirectory: {typeDirectory}");
                    return;
                }

                if (!Directory.Exists(typeDirectory))
                {
                    MyLogger.LogError($"Type directory {typeDirectory} does not exist!");
                    return;
                }

                var filePath = $"{Path.Join(typeDirectory, instance.Name)}.json";
                MyLogger.Log($"Writing {instance.Name} to file: {filePath}");
                await using StreamWriter file   = new StreamWriter(filePath);
                using JsonWriter         writer = new JsonTextWriter(file);

                JsonSerializer.Serialize(writer, instance);
            }
        }

        public void Clear()
        {
            queuedInjections.Clear();
            staticDataDictionary.Clear();
            validationErrors.Clear();

            if (OnStaticDataInstancesBuilt?.GetInvocationList() is var subList
             && subList.IsNullOrEmpty())
            {
                return;
            }

            foreach (var subscriber in subList!)
            {
                OnStaticDataInstancesBuilt -= (Action)subscriber;
            }
        }
    }
}