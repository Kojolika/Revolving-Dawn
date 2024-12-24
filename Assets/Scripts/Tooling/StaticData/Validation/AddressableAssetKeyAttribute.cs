using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Tooling.StaticData.Validation
{
    public class AddressableAssetKeyAttribute : Attribute, IValidationAttribute
    {
        public List<string> errorMessages { get; } = new();

        public bool Validate(Type type, StaticData obj, FieldInfo fieldInfo, List<StaticData> allObjects)
        {
            var fieldValue = fieldInfo.GetValue(obj);
            if (fieldValue is string assetKey)
            {
                var loadHandle = Addressables.LoadResourceLocationsAsync(assetKey);
                loadHandle.WaitForCompletion();
                if (loadHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    errorMessages.Add($"Asset key {assetKey} does not have any assets linked to it. type={type}, staticData={obj}");
                }
            }
            else
            {
                errorMessages.Add($"Field {fieldInfo.Name} is not a string");
                return false;
            }

            return errorMessages.Count == 0;
        }
    }
}