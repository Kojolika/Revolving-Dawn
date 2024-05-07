using System;
using Tooling.Logging;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Utils.Attributes
{
    /// <summary>
    /// Provides editor functionality for selecting addressable assets. We use this over <see cref="UnityEngine.AddressableAssets.AssetReference"/>
    /// because this lets us serialize the addressable key.
    /// NOTE: This attribute MUST be applied to strings as it grabs the addressable key string from the addressable asset.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AddressableAssetAttribute : PropertyAttribute
    {
        public Type Type { get; private set; }

        public AddressableAssetAttribute(Type type)
        {
            if (!typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                throw new ArgumentException($"Type {type} must derive from UnityEngine.Object!");
            }

            Type = type;
        }
    }
}