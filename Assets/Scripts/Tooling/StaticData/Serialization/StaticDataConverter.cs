using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                new JObject(new JProperty(nameof(StaticData), $"{value?.GetType().FullName}.{(value as StaticData)?.Name}"))
                    .WriteTo(writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var staticData = jObject.ToObject(objectType) as StaticData;
            FindStaticDataReferences(jObject, staticData);

            return staticData;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(StaticData).IsAssignableFrom(objectType) && !staticDataTypes.Contains(objectType);
        }

        private void FindStaticDataReferences(JObject jObject,
            StaticData staticData,
            string propertyName = null,
            bool isArray = false,
            int arrayIndex = -1)
        {
            foreach (var prop in jObject.Properties())
            {
                switch (prop.Value.Type)
                {
                    case JTokenType.Object:
                        FindStaticDataReferences((JObject)prop.Value, staticData, prop.Name, isArray, arrayIndex);
                        break;
                    case JTokenType.Array:
                        var jArray = (JArray)prop.Value;
                        for (int i = 0; i < jArray.Count; i++)
                        {
                            FindStaticDataReferences((JObject)jArray[i], staticData, prop.Name, true, i);
                        }

                        break;
                }
            }

            if (!jObject.ContainsKey(nameof(StaticData)))
            {
                return;
            }

            var stringReferenceValue = jObject[nameof(StaticData)]!.Value<string>();
            var referenceSplit = stringReferenceValue.Split(".");
            var typeWithNamespace = referenceSplit[..^1].Aggregate((string1, string2) => $"{string1}.{string2}");
            var staticDataType = Type.GetType(typeWithNamespace);
            var instanceName = referenceSplit[^1];

            if (isArray)
            {
                StaticDatabase.Instance.QueueReferenceInArrayForInject(
                    staticDataType,
                    instanceName,
                    staticData,
                    propertyName,
                    arrayIndex
                );
            }
            else
            {
                StaticDatabase.Instance.QueueReferenceForInject(
                    staticDataType,
                    instanceName,
                    staticData,
                    propertyName
                );
            }
        }
    }
}