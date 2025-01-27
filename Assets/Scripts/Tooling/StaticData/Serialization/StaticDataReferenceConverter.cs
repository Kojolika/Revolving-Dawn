using System;
using Newtonsoft.Json;
using Tooling.Logging;

namespace Tooling.StaticData
{
    public class StaticDataReferenceConverter : JsonConverter
    {
        private const string TypePropertyName = "$type";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var reference = (StaticDataReference)value;

            if (reference == null)
            {
                MyLogger.LogError($"Expected value to be of type {typeof(StaticDataReference)} but value is null");
                return;
            }

            MyLogger.Log($"Writer path: {writer.Path}");

            writer.WriteStartObject();

            // if our static data type has interfaces, we manually write the type of it
            // this lets our deserializer deserialize it to the correct type
            // TODO: figure out if we can determine the field (if any) and only do this if the field is an interface type

            writer.WritePropertyName(TypePropertyName);
            writer.WriteValue(reference.Type.AssemblyQualifiedName);


            //writer.WritePropertyName(nameof(StaticDataReference.Type));
            //writer.WriteValue(reference.Type.AssemblyQualifiedName);

            writer.WritePropertyName(nameof(StaticDataReference.InstanceName));
            writer.WriteValue(reference.InstanceName);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            StaticData instanceWithReference = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    string propertyName = reader.Value as string;

                    // Move to the value of the property
                    reader.Read();

                    if (propertyName == TypePropertyName)
                    {
                        var stringType = reader.Value as string;
                        var type = Type.GetType(stringType);

                        if (type == null)
                        {
                            MyLogger.LogError($"Cannot find type in assembly, {stringType}");
                            return null;
                        }

                        instanceWithReference = Activator.CreateInstance(type) as StaticData;
                    }

                    if (propertyName == nameof(StaticDataReference.InstanceName))
                    {
                        if (instanceWithReference == null)
                        {
                            MyLogger.LogError("Improper JSON, type of the reference must be above the instance name");
                            return null;
                        }

                        var instanceName = reader.Value as string;
                        instanceWithReference.Reference = new StaticDataReference(instanceWithReference.GetType(), instanceName);
                    }
                }
            }


            return instanceWithReference;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(StaticDataReference).IsAssignableFrom(objectType);
        }
    }
}