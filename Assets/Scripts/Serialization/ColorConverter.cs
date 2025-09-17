using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Tooling.Logging;
using UnityEngine;

namespace Serialization
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override Color ReadJson(JsonReader reader,
            Type objectType,
            Color existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var props = JObject.Load(reader)
                .Properties()
                .Select(prop => prop.Value.Value<float>())
                .ToList();
            
            // since we custom serialize only the r g b a properties in this order, we can deserialize it like this
            return hasExistingValue
                ? new Color(
                    props[0], // r
                    props[1], // g
                    props[2], // b
                    props[3]) // a
                : new Color();
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