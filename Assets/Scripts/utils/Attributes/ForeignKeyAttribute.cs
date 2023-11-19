using System;
using UnityEngine;

namespace Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ForeignKeyAttribute : PropertyAttribute
    {
    }
}