using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tooling.StaticData
{
    /// <summary>
    /// The purpose is to only serialize each static data once and then reference that static data
    /// when deserializing to get the data without copy pasting the same data everywhere.
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
                new JObject(new JProperty(nameof(StaticData), $"{value?.GetType().Name}.{(value as StaticData)?.Name}")).WriteTo(writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (existingValue == null)
            {
                return null;
            }

            var jObject = JObject.Load(reader);
            if (jObject.ContainsKey(nameof(StaticData)))
            {
                var stringReferenceValue = jObject[nameof(StaticData)]!.Value<string>();
                var referenceSplit = stringReferenceValue.Split(".");
                var staticDataType = Type.GetType(referenceSplit[0]);
                var instanceName = referenceSplit[1];
            }
            else
            {
                serializer.Deserialize<StaticData>(reader);
            }

            return serializer.Deserialize<StaticData>(reader);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(StaticData).IsAssignableFrom(objectType) && !staticDataTypes.Contains(objectType);
        }
    }
}