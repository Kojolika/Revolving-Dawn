using System;
using System.Collections.Generic;
using System.Reflection;
using Tooling.Logging;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils.Extensions;

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

                if (loadHandle.Status != AsyncOperationStatus.Succeeded
                    || loadHandle.Result.IsNullOrEmpty())
                {
                    errorMessages.Add($"Asset key {assetKey} does not have any assets linked to it.");
                }
            }
            else if (fieldValue is null)
            {
                errorMessages.Add($"Field {fieldInfo.Name} is null.");
            }
            else
            {
                errorMessages.Add($"Field {fieldInfo.Name} is not a string");
            }

            return errorMessages.Count == 0;
        }
    }
}