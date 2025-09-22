using System.Collections.Generic;
using System.Reflection;


namespace Tooling.StaticData.Validation
{
    public abstract class GenericTypeValidator<T> : IValidator where T : class
    {
        public abstract List<string> errorMessages { get; }

        public bool Validate(System.Type type, Data.StaticData obj, FieldInfo fieldInfo, List<Data.StaticData> allObjects)

        {
            var fieldValue = fieldInfo.GetValue(obj) as T;
            return Validate(fieldValue, allObjects);
        }

        protected abstract bool Validate(T value, List<Data.StaticData> allObjects);

        public bool CanValidate(System.Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
    }
}