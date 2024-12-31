using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tooling.Logging;

namespace Tooling.StaticData
{
    /// <summary>
    /// The purpose is to only serialize each static data once and then reference that static data
    /// when deserializing to get the data without duplicating the same data everywhere.
    /// </summary>
    public class StaticDataConverter : JsonConverter
    {
        private readonly HashSet<Type> staticDataTypes = new();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            MyLogger.Log($"Writing static data, path: {writer.Path}");
            // Only serialize top level objects, so the static data that is in its own file
            if (string.IsNullOrEmpty(writer.Path))
            {
                // Add to this hashset to block this converter from serializing
                // this static data again until it's finished writing the whole file.
                staticDataTypes.Add(value?.GetType());

                // Since CanConvert will be false here, will default to serializing an object
                serializer.Serialize(writer, value);

                // remove it so next time we serialize we use the else statement below instead of this flow
                staticDataTypes.Remove(value?.GetType());
            }
            else
            {
                // Serialize a reference to the static data, so we can load it at runtime
                var staticDataRef = new StaticDataReference(value?.GetType().FullName, (value as StaticData)?.Name);
                serializer.Serialize(writer, staticDataRef);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            StaticData staticData;
            if (string.IsNullOrEmpty(reader.Path))
            {
                staticDataTypes.Add(objectType);

                staticData = serializer.Deserialize(reader, objectType) as StaticData;

                staticDataTypes.Remove(objectType);
            }
            else
            {
                staticData = Activator.CreateInstance(objectType) as StaticData;

                var staticDataReference = serializer.Deserialize<StaticDataReference>(reader);
                StaticDatabase.Instance.QueueReferenceForInject(
                    Type.GetType(staticDataReference.FullTypeName),
                    staticDataReference.InstanceName,
                    staticData,
                    reader.Path
                );
            }

            return staticData;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(StaticData).IsAssignableFrom(objectType) && !staticDataTypes.Contains(objectType);
        }

        private class StaticDataReference
        {
            public readonly string FullTypeName;
            public readonly string InstanceName;

            public StaticDataReference(string fullTypeName, string instanceName)
            {
                FullTypeName = fullTypeName;
                InstanceName = instanceName;
            }
        }
    }
}