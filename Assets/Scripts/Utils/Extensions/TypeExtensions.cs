using System;

namespace Utils.Extensions
{
    public static class TypeExtensions
    {
        public static System.Collections.Generic.List<Type> GetBaseClasses(this Type type)
        {
            var baseClasses = new System.Collections.Generic.List<Type>();

            if (type == null)
            {
                return baseClasses;
            }

            type = type.BaseType;
            while (type != null)
            {
                baseClasses.Add(type);
                type = type.BaseType;
            }

            return baseClasses;
        }
    }
}