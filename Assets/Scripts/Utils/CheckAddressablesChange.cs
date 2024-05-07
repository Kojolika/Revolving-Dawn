using Tooling.Logging;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Utils
{
    [InitializeOnLoad]
    public class CheckAddresablesChange
    {
        static CheckAddresablesChange()
        {
            AddressableAssetSettingsDefaultObject.Settings.OnModification += OnAddressableKeyModification;
        }


        static void OnAddressableKeyModification(AddressableAssetSettings settings, AddressableAssetSettings.ModificationEvent evt, object obj)
        {
            if (evt != AddressableAssetSettings.ModificationEvent.EntryModified)
            {
                return;
            }

            MyLogger.Log($" addressable event: {evt}");
            if (obj is AddressableAssetEntry addressableAssetEntry)
            {
                MyLogger.Log($"guid: {addressableAssetEntry.guid}, entry: {addressableAssetEntry.address}");
            }
        }


        ~CheckAddresablesChange()
        {
            AddressableAssetSettingsDefaultObject.Settings.OnModification -= OnAddressableKeyModification;
        }
    }
}