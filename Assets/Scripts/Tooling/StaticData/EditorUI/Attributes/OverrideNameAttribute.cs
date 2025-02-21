using System;

namespace Tooling.StaticData.EditorUI
{
    /// <summary>
    /// Overrides the display name when drawn by a <see cref="GeneralField"/>'s enum or dropdown.
    /// If this is applied to a class, it will override the type name.
    /// If this is applied to an enum value, it will override the value name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Struct)]
    public class OverrideNameAttribute : Attribute
    {
        public readonly string Name;

        public OverrideNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}