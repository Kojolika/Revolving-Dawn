using System;
using System.Collections.Generic;
using System.Reflection;
using Tooling.Logging;

namespace Tooling.StaticData.Validation
{
    public abstract class TypeValidator<T> : IValidator where T : class
    {
        public abstract List<string> errorMessages { get; }

        public bool Validate(Type type, StaticData obj, FieldInfo fieldInfo, List<StaticData> allObjects)

        {
            var fieldValue = fieldInfo.GetValue(obj) as T;
            return Validate(fieldValue, allObjects);
        }

        protected abstract bool Validate(T value, List<StaticData> allObjects);

        public bool CanValidate(Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
    }
}