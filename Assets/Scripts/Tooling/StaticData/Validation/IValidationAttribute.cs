using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tooling.StaticData.Validation
{
    public interface IValidationAttribute
    {
        /// <summary>
        /// List of error messages if field is not valid.
        /// </summary>
        List<string> errorMessages { get; }

        /// <summary>
        /// Validate the given value from the field.
        /// </summary>
        /// <param name="type">The type this field belongs to</param>
        /// <param name="obj">The instance of the type</param>
        /// <param name="fieldInfo">The FieldInfo for this field</param>
        /// <returns>True if valid.</returns>
        bool Validate(Type type, object obj, FieldInfo fieldInfo);
    }
}