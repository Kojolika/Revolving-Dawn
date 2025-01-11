using System.Collections.Generic;
using Tooling.Logging;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;

namespace Tooling.StaticData.Validation
{
    public class AssetReferenceValidator : TypeValidator<AssetReference>
    {
        public override List<string> errorMessages { get; } = new();

        protected override bool Validate(AssetReference value, List<StaticData> allObjects)
        {
            errorMessages.Clear();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var asset = value?.editorAsset;
            if (asset == null)
            {
                errorMessages.Add("Referenced asset is null");
                return false;
            }

            var entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)));

            var isAssetAddressable = entry != null;
            if (!isAssetAddressable)
            {
                errorMessages.Add("Asset reference is not a valid asset addressable");
            }

            return isAssetAddressable;
        }
    }
}