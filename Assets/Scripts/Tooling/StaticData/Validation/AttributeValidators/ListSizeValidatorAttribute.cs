using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Tooling.StaticData.Validation
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ListSizeValidatorAttribute : Attribute, IValidator
    {
        public List<string> errorMessages { get; } = new();

        private readonly int? minCount;
        private readonly int? maxCount;

        public ListSizeValidatorAttribute(int minCount = -1, int maxCount = -1)
        {
            this.minCount = minCount <= -1
                ? null
                : minCount;

            this.maxCount = maxCount <= -1
                ? null
                : maxCount;
        }

        public bool Validate(Type type, Data.StaticData obj, FieldInfo fieldInfo, List<Data.StaticData> allObjects)
        {
            var list     = fieldInfo.GetValue(obj) as IList;
            int listSize = list?.Count ?? 0;

            if (listSize < minCount)
            {
                errorMessages.Add($"Must have a list count greater than {minCount}! Current count is {listSize}");
                return false;
            }

            if (listSize > maxCount)
            {
                errorMessages.Add($"Must have a list count less than {maxCount}! Current count is {listSize}");
                return false;
            }

            return true;
        }

        public bool CanValidate(Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }
    }
}