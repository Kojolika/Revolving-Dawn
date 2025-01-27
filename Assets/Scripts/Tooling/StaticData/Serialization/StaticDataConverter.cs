using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serialization;
using Tooling.Logging;

namespace Tooling.StaticData
{
    /// <summary>
    /// The purpose is to only serialize each static data once and then reference that static data
    /// when deserializing to get the data without duplicating the same data everywhere.
    /// </summary>
    public class StaticDataConverter : JsonConverter<StaticData>
    {
        private const string TypePropertyName = "$type";


        public override void WriteJson(JsonWriter writer, StaticData value, JsonSerializer serializer)
        {
            var staticDataType = value.GetType();

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
                var staticDataRef = new StaticDataReference(staticDataType, value.Name);
                if (staticDataRef.Type == null)
                {
                    MyLogger.LogError($"Trying to serialize a reference to object type {staticDataType} but" +
                                      $"The {nameof(StaticDataReference.Type)} of this reference is null!");

                    return;
                }

                if (string.IsNullOrEmpty(staticDataRef.InstanceName))
                {
                    MyLogger.LogError($"Trying to serialize a reference to object type {staticDataType} but" +
                                      $"The {nameof(StaticDataReference.InstanceName)} of this reference is null!");

                    return;
                }

                writer.WriteStartObject();
                {
                    writer.WritePropertyName(TypePropertyName);
                    writer.WriteValue(staticDataRef.Type.AssemblyQualifiedName);

                    writer.WritePropertyName(nameof(StaticData.Reference));

                    writer.WriteStartObject();
                    { 
                        writer.WritePropertyName(nameof(StaticDataReference.Type));
                        writer.WriteValue(staticDataRef.Type.AssemblyQualifiedName);

                        writer.WritePropertyName(nameof(StaticDataReference.InstanceName));
                        writer.WriteValue(staticDataRef.InstanceName);
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }
        }

        public override StaticData ReadJson(JsonReader reader,
            Type objectType,
            StaticData existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            //MyLogger.Log($"Reading static data type: {objectType}");
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
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = reader.Value as string;

                        // Move to the value of the property
                        reader.Read();

                        /*if (propertyName == TypePropertyName)
                        {
                            var stringType = reader.Value as string ?? string.Empty;
                            var type = Type.GetType(stringType);

                            if (type == null)
                            {
                                MyLogger.LogError($"Cannot find type in assembly, {stringType}");
                                return null;
                            }

                            //staticData = Activator.CreateInstance(type) as StaticData;
                        }*/

                        if (propertyName == nameof(StaticDataReference.InstanceName))
                        {
                            if (staticData == null)
                            {
                                MyLogger.LogError("Improper JSON, type of the reference must be above the instance name");
                                return null;
                            }

                            var instanceName = reader.Value as string;
                            MyLogger.Log($"Found instance name :{instanceName} for reference type {objectType}");
                            staticData.Reference = new StaticDataReference(objectType, instanceName);
                        }
                    }
                }
            }

            return staticData;
        }
    }
}