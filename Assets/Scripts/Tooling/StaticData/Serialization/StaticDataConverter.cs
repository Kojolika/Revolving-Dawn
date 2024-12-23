using System;
using Newtonsoft.Json;

namespace Tooling.StaticData.Serialization
{
    public class StaticDataConverter : JsonConverter<StaticData>
    {
        public override void WriteJson(JsonWriter writer, StaticData value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, $"{value.GetType().Name}.{value.Name}");
        }

        public override StaticData ReadJson(JsonReader reader,
            Type objectType,
            StaticData existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
            
        }
    }
}