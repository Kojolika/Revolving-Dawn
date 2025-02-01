using System;

namespace Tooling.StaticData.Attributes
{
    /// <summary>
    /// The <see cref="GeneralField"/> will ignore drawing editing fields for fields with this attribute or if a class has this attribute
    /// it will not be drawn for any editor in the <see cref="GeneralField"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Interface)]
    public class GeneralFieldIgnoreAttribute : Attribute
    {
    }
}