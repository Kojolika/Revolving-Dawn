using System;
using UnityEngine;

namespace Utils.Attributes
{
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