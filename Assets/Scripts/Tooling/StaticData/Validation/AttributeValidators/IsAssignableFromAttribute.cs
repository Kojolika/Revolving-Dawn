using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tooling.StaticData.EditorUI.Validation
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

        public bool Validate(Type type, StaticData obj, FieldInfo fieldInfo, List<StaticData> allObjects)
        {
            var typeToCheck = fieldInfo.GetValue(obj) as Type;
            if (typeToCheck == null)
            {
                errorMessages.Add("Type cannot be null!");
                return false;
            }

            var isValid = this.TargetType.IsAssignableFrom(typeToCheck);
            if (!isValid)
            {
                errorMessages.Add($"Type {typeToCheck} is not assignable to " + this.TargetType);
                return false;
            }

            return true;
        }

        public bool CanValidate(Type type)
        {
            return true;
        }
    }
}