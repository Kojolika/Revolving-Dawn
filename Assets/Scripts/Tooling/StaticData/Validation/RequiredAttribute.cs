using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Tooling.StaticData.Validation
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : Attribute, IValidationAttribute
    {
        /// <summary>
        /// Can this value be the default value for the field type?
        /// <remarks>Only used for <c>value</c> types.</remarks>
        /// </summary>
        private bool allowDefaultValues;

        public List<string> errorMessages { get; } = new();

        public RequiredAttribute(bool allowDefaultValues)
        {
            this.allowDefaultValues = allowDefaultValues;
        }

        public bool Validate(Type type, StaticData obj, FieldInfo fieldInfo, List<StaticData> allObjects)
        {
            if (fieldInfo.FieldType.IsClass && fieldInfo.GetValue(obj) == null)
            {
                errorMessages.Add("Field value cannot be null!");
                return false;
            }

            if (!allowDefaultValues
                && fieldInfo.FieldType.IsValueType
                && Activator.CreateInstance(fieldInfo.FieldType) is var valueInstance
                && fieldInfo.GetValue(obj) == valueInstance)
            {
                errorMessages.Add($"Field value cannot be default! " +
                                  $"In this behaviour is desired, set {nameof(allowDefaultValues)} to true.");
                return false;
            }

            if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType) && ((IList)fieldInfo.GetValue(obj)).Count == 0)
            {
                errorMessages.Add("Field value cannot be an empty list! ");
            }

            return true;
        }
    }
}