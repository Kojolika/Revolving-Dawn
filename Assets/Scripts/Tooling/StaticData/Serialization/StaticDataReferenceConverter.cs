using System;
using Newtonsoft.Json;

namespace Tooling.StaticData
{
    public class StaticDataReferenceConverter : JsonConverter<StaticDataReference>
    {
        public override void WriteJson(JsonWriter writer, StaticDataReference value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            // if our static data type has interfaces, we manually write the type of it
            // this lets our deserializer deserialize it to the correct type
            // TODO: figure out if we can determine the field (if any) and only do this if the field is an interface type
            if (value.Type.GetInterfaces().Length > 0)
            {
                writer.WritePropertyName("$type");
                writer.WriteValue(value.Type.AssemblyQualifiedName);
            }

            writer.WritePropertyName(nameof(StaticDataReference.Type));
            writer.WriteValue(value.Type.AssemblyQualifiedName);

            writer.WritePropertyName(nameof(StaticDataReference.InstanceName));
            writer.WriteValue(value.InstanceName);

            writer.WriteEndObject();
        }

        public override StaticDataReference ReadJson(JsonReader reader,
            Type objectType,
            StaticDataReference existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => false;
    }
}