using System;
using Tooling.Logging;
using UnityEngine;

namespace Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ForeignKeyAttribute : PropertyAttribute
    {
        public Type type;

        public ForeignKeyAttribute(Type type){
            if(!typeof(ScriptableObject).IsAssignableFrom(type)){
                MyLogger.LogError($"Type {type} must be assignable from type {nameof(ScriptableObject)}");
            }
            this.type = type;
        }
    }
}