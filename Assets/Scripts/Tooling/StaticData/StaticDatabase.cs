using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Serialization;
using Tooling.Logging;
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

        public readonly JsonSerializer JsonSerializer = new()
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CustomContractResolver(),
            Converters =
            {
                new AssetReferenceConverter(),
                new ColorConverter(),
                new StaticDataConverter()
            },
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
        };

        private readonly List<StaticDataReferenceHandle> queuedInjections = new();

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

        private StaticDatabase()
        {
        }

        public void BuildDictionaryFromJson()
        {
            var staticDataTypes = typeof(StaticData).Assembly.GetTypes()
                .Where(type => typeof(StaticData).IsAssignableFrom(type) && !type.IsAbstract)
                .ToList();

            queuedInjections.Clear();
            staticDataDictionary.Clear();

            // If we hot reload while the window is open and a new static data type is created, add to our dict
            foreach (var type in staticDataTypes)
            {
                if (staticDataDictionary.ContainsKey(type))
                {
                    continue;
                }

                var instanceDictionary = new Dictionary<string, StaticData>();
                var typeDirectory = Path.GetFullPath(Path.Join(StaticDataDirectory, type.Name));

                if (Directory.Exists(typeDirectory))
                {
                    foreach (var file in Directory.EnumerateFiles(typeDirectory, "*.json"))
                    {
                        using var streamReader = File.OpenText(file);

                        var staticDataFromJson = (StaticData)JsonSerializer.Deserialize(streamReader, type);
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
        public void QueueReferenceForInject(
            Type staticDataType,
            string instanceName,
            StaticData staticData,
            string propertyName)
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
            queuedInjections.Add(new StaticDataReferenceHandle(staticDataType, instanceName, staticData, propertyName, index));
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
                        GetStaticData(referenceHandle.Type, referenceHandle.InstanceName)
                    );
                }
                else
                {
                    var list = (IList)(staticDataField.GetValue(referenceHandle.ObjectWithReference)
                                       ?? Activator.CreateInstance(staticDataField.FieldType));
                    list[referenceHandle.ArrayIndex] = GetStaticData(referenceHandle.Type, referenceHandle.InstanceName);

                    staticDataField.SetValue(referenceHandle.ObjectWithReference, list);
                }
            }
        }

        public StaticData GetStaticData(Type type, string instanceName)
        {
            if (staticDataDictionary.TryGetValue(type, out var instanceDictionary)
                && instanceDictionary.TryGetValue(instanceName, out var dataInstance))
            {
                return dataInstance;
            }

            return null;
        }

        public void Clear()
        {
            queuedInjections.Clear();
            staticDataDictionary.Clear();
        }
    }
}