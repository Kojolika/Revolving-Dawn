using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Serialization;
using Tooling.Logging;
using Tooling.StaticData.Data.Validation;
using UnityEditor;
using UnityEngine;
using Utils.Extensions;

namespace Tooling.StaticData.Data
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

        public bool HasStaticDataInstanceBeenBuilt { get; private set; }

        public event Action       StaticDataInstancesBuilt;
        public event Action<Type> InstancesUpdated;

        public Dictionary<Type, Dictionary<StaticData, List<string>>> ValidationErrors { get; private set; } = new();

        /// <summary>
        /// Notifies when validation has just been completed. The <see cref="ValidationErrors"/> will be populated after
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
                new StaticDataConverter()
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
            HasStaticDataInstanceBeenBuilt = false;
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
                    foreach (var filePath in Directory.EnumerateFiles(typeDirectory, "*.json"))
                    {
                        using var streamReader = File.OpenText(filePath);

                        StaticData staticDataFromJson = null;

                        try
                        {
                            staticDataFromJson = (StaticData)JsonSerializer.Deserialize(streamReader, type);
                        }
                        catch (Exception e)
                        {
                            MyLogger.Error($"Exception while deserializing file {Path.GetFileName(filePath)} of type {type.Name}: {e.Message}\n {e}");
                            staticDataFromJson = Activator.CreateInstance(type) as StaticData;
                        }

                        if (staticDataFromJson == null)
                        {
                            MyLogger.Error("Static data is still null, is the type wrong...?");
                            continue;
                        }

                        var fileNameWithExtension = new FileInfo(filePath).Name;
                        staticDataFromJson.Name = fileNameWithExtension[..^".json".Length];

                        instanceDictionary.Add(staticDataFromJson.Name, staticDataFromJson);
                    }
                }

                staticDataDictionary.Add(type, instanceDictionary);
            }

            HasStaticDataInstanceBeenBuilt = true;
            InjectReferences();
        }

        /// <summary>
        /// Queues an object to have its references to <see cref="StaticData"/> injected
        /// after every <see cref="StaticData"/> is deserialized.
        /// </summary>
        public void QueueReferenceForInject(
            Type       referenceType,
            string     instanceName,
            object     objectWithReference,
            string     propertyName,
            MemberType memberType,
            int        arrayIndex = -1)
        {
            if (HasStaticDataInstanceBeenBuilt)
            {
                InjectReference(
                    referenceType,
                    instanceName,
                    objectWithReference,
                    propertyName,
                    memberType,
                    arrayIndex);
            }
            else
            {
                queuedInjections.Add(new StaticDataReferenceHandle(referenceType, instanceName, objectWithReference, propertyName, memberType, arrayIndex));
            }
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
                InjectReference(
                    referenceHandle.ReferenceType,
                    referenceHandle.InstanceName,
                    referenceHandle.ObjectWithReference,
                    referenceHandle.PropertyName,
                    referenceHandle.MemberType,
                    referenceHandle.ArrayIndex);
            }

            queuedInjections.Clear();
        }

        private void InjectReference(
            Type       referenceType,
            string     instanceName,
            object     objectWithReference,
            string     memberName,
            MemberType memberType,
            int        arrayIndex = -1)
        {
            switch (memberType)
            {
                case MemberType.Field:
                    InjectField(referenceType, instanceName, objectWithReference, memberName, arrayIndex);
                    break;
                case MemberType.Property:
                    InjectProperty(referenceType, instanceName, objectWithReference, memberName, arrayIndex);
                    break;
            }
        }

        private void InjectField(
            Type   referenceType,
            string instanceName,
            object objectWithReference,
            string fieldName,
            int    arrayIndex = -1)
        {
            var       objType         = objectWithReference.GetType();
            FieldInfo staticDataField = EditorUI.Utils.GetField(objType, fieldName);
            if (staticDataField == null)
            {
                // If we match a property backing field, don't log error since we check properties in InjectProperty
                if (Regex.Match(fieldName, "<.+>k__BackingField").Success)
                {
                    return;
                }

                MyLogger.Error($"Could not find field {fieldName} of type {objType}");
                return;
            }

            if (!typeof(IList).IsAssignableFrom(staticDataField.FieldType) && staticDataField.FieldType != referenceType)
            {
                MyLogger.Error("Field that is requesting to be filled in is not equal to the type that's referenced! " +
                               $"referenceType={referenceType}, fieldType={staticDataField.FieldType}, objectWithReference={objType}, propertyName={fieldName}");
                return;
            }

            StaticData referencedInstance = GetInstance(referenceType, instanceName);
            if (referencedInstance == null)
            {
                MyLogger.Error($"Trying to find {referenceType} with name {instanceName}," +
                               $" but does not exist in the StaticDatabase. Requesting type={objectWithReference}");
                return;
            }

            if (arrayIndex < 0)
            {
                staticDataField!.SetValue(objectWithReference, referencedInstance);
            }
            else if (typeof(IList).IsAssignableFrom(staticDataField.FieldType))
            {
                var list = (IList)(staticDataField!.GetValue(objectWithReference)
                                ?? Activator.CreateInstance(staticDataField.FieldType));
                list[arrayIndex] = referencedInstance;
                staticDataField.SetValue(objectWithReference, list);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(staticDataField.FieldType))
            {
                var enumerable = (IEnumerable)(staticDataField!.GetValue(objectWithReference)
                                            ?? Activator.CreateInstance(staticDataField.FieldType));

                var list = enumerable.Cast<object>().ToList();
                list[arrayIndex] = referencedInstance;
                staticDataField.SetValue(objectWithReference, (IEnumerable)list);
            }
        }

        private void InjectProperty(
            Type   referenceType,
            string instanceName,
            object objectWithReference,
            string propertyName,
            int    arrayIndex = -1)
        {
            var objType = objectWithReference.GetType();

            PropertyInfo staticDataProperty = objType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (staticDataProperty == null)
            {
                MyLogger.Error($"Could not find property {propertyName} on Static Data of type {objType}");
                return;
            }

            // No set method for this property, then return... since we can't set it
            if (staticDataProperty.GetSetMethod(true) == null)
            {
                return;
            }

            // Property could be a list of the type we have
            if (!typeof(IList).IsAssignableFrom(staticDataProperty.PropertyType) && staticDataProperty.PropertyType != referenceType)
            {
                MyLogger.Error("Property that is requesting to be filled in is not equal to the type that's referenced! " +
                               $"referenceType={referenceType}, propertyType={staticDataProperty.PropertyType}, objectWithReference={objType}, propertyName={propertyName}");
                return;
            }


            StaticData referencedInstance = GetInstance(referenceType, instanceName);
            if (referencedInstance == null)
            {
                MyLogger.Error($"Trying to find {referenceType} with name {instanceName}," +
                               $" but does not exist in the StaticDatabase. Requesting type={objectWithReference}");
                return;
            }

            if (arrayIndex < 0)
            {
                staticDataProperty.SetValue(objectWithReference, referencedInstance);
            }
            else if (typeof(IList).IsAssignableFrom(staticDataProperty.PropertyType))
            {
                var list = (IList)(staticDataProperty.GetValue(objectWithReference)
                                ?? Activator.CreateInstance(staticDataProperty.PropertyType));
                list[arrayIndex] = referencedInstance;
                staticDataProperty.SetValue(objectWithReference, list);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(staticDataProperty.PropertyType))
            {
                var enumerable = (IEnumerable)(staticDataProperty!.GetValue(objectWithReference)
                                            ?? Activator.CreateInstance(staticDataProperty.PropertyType));

                var list = enumerable.Cast<object>().ToList();
                list[arrayIndex] = referencedInstance;
                staticDataProperty.SetValue(objectWithReference, (IEnumerable)list);
            }
        }

        private struct StaticDataReferenceHandle
        {
            /// <summary>
            /// The type of static data being referenced.
            /// </summary>
            public readonly Type ReferenceType;

            /// <summary>
            /// The instance name of the <see cref="ReferenceType"/> being referenced.
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
            /// Specifies whether the propertyName refers to a field or a property.
            /// </summary>
            public readonly MemberType MemberType;

            /// <summary>
            /// If the property is an array, this will be set to a value > -1.
            /// </summary>
            public readonly int ArrayIndex;

            public StaticDataReferenceHandle(
                Type       referenceType,
                string     instanceName,
                object     objectWithReference,
                string     propertyName,
                MemberType memberMemberType,
                int        arrayIndex = -1)
            {
                ReferenceType       = referenceType;
                InstanceName        = instanceName;
                ObjectWithReference = objectWithReference;
                PropertyName        = propertyName;
                MemberType          = memberMemberType;
                ArrayIndex          = arrayIndex;
            }
        }

        public enum MemberType
        {
            /// <summary>
            /// This value is from a field
            /// </summary>
            Field,

            /// <summary>
            /// This value is from a property
            /// </summary>
            Property
        }

        public void ValidateStaticData()
        {
            ValidationErrors = validator.ValidateObjects(GetAllStaticDataInstances());
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

        public StaticData GetInstance(Type type, string instanceName)
        {
            if (staticDataDictionary.TryGetValue(type, out var instanceDictionary)
             && instanceDictionary.TryGetValue(instanceName, out var dataInstance))
            {
                return dataInstance;
            }

            return null;
        }

        public T GetInstance<T>(string instanceName) where T : StaticData
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
                MyLogger.Error($"Error saving to json: {e}");
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
                    MyLogger.Error($"Trying to serialize null StaticData in typeDirectory: {typeDirectory}");
                    return;
                }

                if (!Directory.Exists(typeDirectory))
                {
                    MyLogger.Error($"Type directory {typeDirectory} does not exist!");
                    return;
                }

                var filePath = $"{Path.Join(typeDirectory, instance.Name)}.json";
                MyLogger.Info($"Writing {instance.Name} to file: {filePath}");
                await using StreamWriter file   = new StreamWriter(filePath);
                using JsonWriter         writer = new JsonTextWriter(file);

                JsonSerializer.Serialize(writer, instance);
            }
        }

        public void Clear()
        {
            queuedInjections.Clear();
            staticDataDictionary.Clear();
            ValidationErrors.Clear();

            if (StaticDataInstancesBuilt?.GetInvocationList() is var subList
             && subList.IsNullOrEmpty())
            {
                return;
            }

            foreach (var subscriber in subList!)
            {
                StaticDataInstancesBuilt -= (Action)subscriber;
            }
        }
    }
}