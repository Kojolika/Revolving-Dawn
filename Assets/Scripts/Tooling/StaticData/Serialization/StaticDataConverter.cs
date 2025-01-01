using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
                if (string.IsNullOrEmpty(staticDataRef.FullTypeName))
                {
                    MyLogger.LogError($"Trying to serialize a reference to object type {value?.GetType()} but" +
                                      $"The FullTypeName of this reference is null!");
                }

                if (string.IsNullOrEmpty(staticDataRef.InstanceName))
                {
                    MyLogger.LogError($"Trying to serialize a reference to object type {value?.GetType()} but" +
                                      $"The InstanceName of this reference is null!");
                }

                serializer.Serialize(writer, staticDataRef);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            StaticData staticData;
            if (string.IsNullOrEmpty(reader.Path))
            {
                // see WriteJson for why we do this
                staticDataTypes.Add(objectType);

                staticData = serializer.Deserialize(reader, objectType) as StaticData;

                staticDataTypes.Remove(objectType);
            }
            else
            {
                var staticDataReference = serializer.Deserialize<StaticDataReference>(reader);
                if (staticDataReference == null)
                {
                    return null;
                }

                staticData = Activator.CreateInstance(objectType) as StaticData;
                staticData!.SerializedReference = staticDataReference;
            }

            return staticData;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(StaticData).IsAssignableFrom(objectType) && !staticDataTypes.Contains(objectType);
        }
    }
}