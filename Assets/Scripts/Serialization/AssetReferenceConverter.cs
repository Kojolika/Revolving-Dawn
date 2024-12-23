using System;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;

namespace Serialization
{
    public class AssetReferenceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => typeof(AssetReference).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var assetGuid = (string)reader.Value;
            return Activator.CreateInstance(objectType, assetGuid);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((value as AssetReference)!.AssetGUID);
        }
    }
}