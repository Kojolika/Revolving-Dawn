using System;
using System.Reflection;
using Newtonsoft.Json;
using Tooling.Logging;

namespace Tooling.StaticData.Data
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
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var staticDataType = value.GetType();


            // Only serialize top level objects, so the static data that is in its own file
            if (string.IsNullOrEmpty(writer.Path))
            {
                writer.WriteStartObject();

                var fields = Utils.GetFields(staticDataType);
                foreach (var field in fields)
                {
                    // we custom serialize references below
                    if (field.Name == nameof(StaticData.Reference) ||
                        field.GetCustomAttribute<JsonIgnoreAttribute>() != null)
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
                if (string.IsNullOrEmpty(staticDataRef.InstanceName))
                {
                    MyLogger.Error($"Trying to serialize a reference to object type {staticDataType} but " +
                                      $"the {nameof(StaticDataReference.InstanceName)} of this reference is null!");

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

        public override bool CanRead => false;

        public override StaticData ReadJson(JsonReader reader,
            System.Type objectType,
            StaticData existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}