using System;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;

namespace Serialization
{
    public class AssetReferenceConverter : JsonConverter<AssetReference>
    {
        public override AssetReference ReadJson(JsonReader reader, Type objectType, AssetReference existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var assetGuid = (string)reader.Value;
            AssetReference assetReference = new AssetReference(assetGuid);

            return assetReference;
        }

        public override void WriteJson(JsonWriter writer, AssetReference value, JsonSerializer serializer)
        {
            writer.WriteValue(value.AssetGUID);
        }
    }
}