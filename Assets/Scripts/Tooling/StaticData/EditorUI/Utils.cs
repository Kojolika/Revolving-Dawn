using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tooling.StaticData.Data.EditorUI;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Extensions;

namespace Tooling.StaticData.Data
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

        /// <summary>
        /// If our type is a static data type, sort the fields to have the name at the top while retaining the order of the rest of the fields
        /// </summary>
        public static void SortFields(List<FieldInfo> fields, Type type)
        {
            if (!typeof(StaticData).IsAssignableFrom(type) || fields.Count <= 1)
            {
                return;
            }

            var nameField = fields.First(field => field.Name == nameof(StaticData.Name));
            var nameIndex = fields.IndexOf(nameField);
            for (int i = fields.Count - 1; i > 0; i--)
            {
                if (i > nameIndex)
                {
                    continue;
                }

                fields[i] = fields[i - 1];
            }

            fields[0] = nameField;
        }

        /// <summary>
        /// Adds a label to a parent for any field value
        /// </summary>
        public static void AddLabel(VisualElement root, string text, string tooltip)
        {
            if (text.IsNullOrEmpty())
            {
                return;
            }

            var label = new Label(text) { tooltip = tooltip };
            label.AddToClassList("unity-base-field__label");
            root.Add(label);
        }

        /// <summary>
        /// Returns whether a value provider has an attribute from its source
        /// Currently only useful for <see cref="FieldValueProvider"/>... but will leave in as a util function
        /// </summary>
        public static bool HasAttribute<T>(IValueProvider valueProvider, out T attribute) where T : Attribute
        {
            if (valueProvider is not FieldValueProvider fieldValueProvider)
            {
                attribute = null;
                return false;
            }

            attribute = fieldValueProvider.FieldInfo.GetCustomAttribute<T>();
            return attribute != null;
        }

        public static string GetTooltip(IValueProvider valueProvider)
        {
            return HasAttribute(valueProvider, out TooltipAttribute attribute)
                ? attribute.tooltip
                : string.Empty;
        }
    }
}