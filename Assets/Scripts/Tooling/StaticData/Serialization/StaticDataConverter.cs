using System;
using System.Reflection;
using Newtonsoft.Json;
using Serialization;
using Tooling.Logging;

namespace Tooling.StaticData
{
    /// <summary>
    /// The purpose is to only serialize each static data once and then reference that static data
    /// when deserializing to get the data without duplicating the same data everywhere.
    /// </summary>
    public class StaticDataConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var staticDataType = value!.GetType();
 
            // Only serialize top level objects, so the static data that is in its own file
            if (string.IsNullOrEmpty(writer.Path))
            {
                writer.WriteStartObject();

                var fields = Utils.GetFields(staticDataType);
                foreach (var field in fields)
                {
                    if (field.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                    {
                        continue;
                    }

                    writer.WritePropertyName(field.Name);

                    serializer.Serialize(writer, field.GetValue(value), field.FieldType);
                }

                writer.WriteEndObject();
            }
            else
            {
                // Serialize a reference to the static data, so we can load it at runtime
                var staticDataRef = new StaticDataReference(staticDataType, (value as StaticData)!.Name);
                if (staticDataRef.Type == null)
                {
                    MyLogger.LogError($"Trying to serialize a reference to object type {staticDataType} but" +
                                      $"The {nameof(StaticDataReference.Type)} of this reference is null!");
                }

                if (string.IsNullOrEmpty(staticDataRef.InstanceName))
                {
                    MyLogger.LogError($"Trying to serialize a reference to object type {staticDataType} but" +
                                      $"The {nameof(StaticDataReference.InstanceName)} of this reference is null!");
                }

                serializer.Serialize(writer, staticDataRef);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var staticData = Activator.CreateInstance(objectType) as StaticData;
            if (string.IsNullOrEmpty(reader.Path))
            {
                do
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            var propertyName = reader.Value as string;
                            // Move reader to the prop value
                            reader.Read();

                            var field = objectType.GetField(propertyName);
                            var fieldValue = serializer.Deserialize(reader, field.FieldType);
                            field.SetValue(staticData, fieldValue);

                            break;
                    }
                } while (reader.Read());

                // This is an OnDeserializedCallback defined in CustomContractResolver
                // since we are manually looping through the properties, we have to invoke it ourselves here
                CustomContractResolver.RecursivelyResolveStaticDataReferences(staticData);
            }
            else
            {
                var staticDataReference = serializer.Deserialize<StaticDataReference>(reader);
                if (staticDataReference == null)
                {
                    return null;
                }

                staticData!.SerializedReference = staticDataReference;
            }

            return staticData;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(StaticData).IsAssignableFrom(objectType);
        }
    }
}