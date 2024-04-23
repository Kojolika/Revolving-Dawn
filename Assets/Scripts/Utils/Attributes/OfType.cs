namespace Utils.Attributes
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Specifies types that an Object needs to be of. Can be used to create an Object selector that allows interfaces.
    /// </summary>
    public class OfTypeAttribute : PropertyAttribute
    {
        public Type[] types;

        public OfTypeAttribute(params Type[] types)
        {
            this.types = types;
        }
    }
}