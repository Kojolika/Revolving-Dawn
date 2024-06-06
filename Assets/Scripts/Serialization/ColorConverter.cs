using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tooling.Logging;
using UnityEngine;

namespace Serialization
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            MyLogger.Log($"Reader val: {reader.Value}");
            return new Color();
        }

        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            var jObject = new JObject
            {
                new JProperty("r", value.r),
                new JProperty("g", value.g),
                new JProperty("b", value.b),
                new JProperty("a", value.a)
            };
            jObject.WriteTo(writer);
        }
    }
}