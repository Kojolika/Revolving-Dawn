using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                .Where(field => field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
                .ToList();

            return fields;
        }

        /// <summary>
        /// Grabs the field with the specified name that is displayable in the <see cref="EditorUI.EditorWindow"/>
        /// </summary>
        public static FieldInfo GetField(Type type, string fieldName)
        {
            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(field => field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
                .FirstOrDefault(field => field.Name == fieldName);
        }
    }
}