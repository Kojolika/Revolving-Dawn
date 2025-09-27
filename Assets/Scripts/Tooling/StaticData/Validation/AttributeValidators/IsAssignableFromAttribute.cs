using System;
using System.Collections.Generic;
using System.Reflection;
using Tooling.Logging;
using UnityEngine;

namespace Tooling.StaticData.Validation
{
    /// <summary>
    /// Add this to a field to ensure that it's condition is valid
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IsAssignableFromAttribute : Attribute, IValidator
    {
        public List<string> errorMessages { get; } = new();

        public readonly Type TargetType;

        public IsAssignableFromAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        public bool Validate(Type type, Data.StaticData obj, FieldInfo fieldInfo, List<Data.StaticData> allObjects)
        {
            var fieldType = fieldInfo.GetValue(obj);
            if (fieldType is Type typeToCheck && TargetType.IsAssignableFrom(typeToCheck))
            {
                return true;
            }

            if (fieldType is IEnumerable<Type> typeList)
            {
                int errors = 0;
                foreach (var t in typeList)
                {
                    if (!TargetType.IsAssignableFrom(t))
                    {
                        errors++;
                    }
                }

                if (errors == 0)
                {
                    return true;
                }

                errorMessages.Add($"Field {fieldInfo.Name} has {errors} elements that are not a valid type. They should be assignable from {TargetType}.");
                return false;
            }

            if (fieldType == null)
            {
                errorMessages.Add("Type cannot be null!");
                return false;
            }
            else
            {
                errorMessages.Add("Field is invalid type");
                return false;
            }
        }

        public bool CanValidate(Type type)
        {
            return true;
        }
    }
}