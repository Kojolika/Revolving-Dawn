using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tooling.StaticData.EditorUI;
using UnityEngine;

namespace Tooling.StaticData
{
    public static class Utils
    {
        /// <summary>
        /// Grabs the fields that are displayable in the <see cref="EditorUI.EditorWindow"/>
        /// </summary>
        public static List<FieldInfo> GetFields(Type type)
        {
            var fields = new List<FieldInfo>();

            if (type == null)
            {
                return fields;
            }

            fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(IsDrawable)
                .ToList();

            return fields;
        }

        /// <summary>
        /// Grabs the field with the specified name that is displayable in the <see cref="EditorUI.EditorWindow"/>
        /// </summary>
        public static FieldInfo GetField(Type type, string fieldName)
        {
            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(IsDrawable)
                .FirstOrDefault(field => field.Name == fieldName);
        }


        /// <summary>
        /// The filter to use to determine if we should draw a field.
        /// </summary>
        private static bool IsDrawable(FieldInfo field)
        {
            return (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
                   && field.GetCustomAttribute<GeneralFieldIgnoreAttribute>()?.IgnoreType != IgnoreType.Field;
        }
    }
}