using System;
using UnityEngine;

namespace Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PrimaryKeyAttribute : PropertyAttribute
    {
    }
}