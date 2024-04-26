using System;
using Tooling.Logging;
using UnityEngine;


namespace Utils.Attributes
{
    /// <summary>
    /// Add this to interface fields in the inspector to display them.
    /// Note: the field MUST also have the <see cref="UnityEngine.SerializeReference"/> attribute
    /// as well to display it correctly.
    /// Furthurmore, due to the restraints of the <see cref="UnityEngine.SerializeReference"/> attribute,
    /// and the restraints of generics not being allowed for attributes.
    /// If a list of the interface is needed, wrap the interface in a class with a field which is the interface.
    /// Then create a list of that class wrapper to view in the inspector. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DisplayInterfaceAttribute : PropertyAttribute
    {
        public Type Type { get; private set; }

        public DisplayInterfaceAttribute(Type type)
        {
            if (!type.IsInterface)
            {
                throw new ArgumentException($"Type {type} must be an interface!");
            }

            Type = type;
        }
    }
}