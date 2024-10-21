using System;
using UnityEngine;


namespace Utils.Attributes
{
    /// <summary>
    /// Add this to abstract or interface fields in the inspector to display them.
    /// Note: the field MUST also have the <see cref="SerializeReference"/> attribute
    /// as well to display it correctly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DisplayAbstractAttribute : PropertyAttribute
    {
        public Type Type { get; private set; }

        public DisplayAbstractAttribute(Type type)
        {
            if (!type.IsInterface && !type.IsAbstract)
            {
                throw new ArgumentException($"Type {type} must be an interface or abstract type!");
            }

            Type = type;
        }
    }
}