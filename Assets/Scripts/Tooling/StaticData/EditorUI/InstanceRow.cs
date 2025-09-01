using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Common.Util;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Extensions;

namespace Tooling.StaticData.EditorUI.EditorUI
{
    /// <summary>
    /// Displays a single row of a static data instance
    /// </summary>
    public class InstanceRow : VisualElement
    {
        private readonly VisualElement row;
        private readonly Type          staticDataType;
        private readonly bool          allowEditing;

        public const float EditButtonWidth = 32;

        private StaticData instance;

        private Dictionary<StaticData, List<string>> validationErrors;

        public InstanceRow(Type staticDataType, bool allowEditing)
        {
            this.staticDataType = staticDataType;
            this.allowEditing   = allowEditing;

            row = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row }
            };


            Add(row);

            CheckForValidationErrors();
            StaticDatabase.Instance.ValidationCompleted += CheckForValidationErrors;
        }

        ~InstanceRow()
        {
            StaticDatabase.Instance.ValidationCompleted -= CheckForValidationErrors;
        }

        private void CheckForValidationErrors()
        {
            StaticDatabase.Instance.validationErrors.TryGetValue(staticDataType, out validationErrors);
            RefreshView();
        }

        public void BindItem(StaticData instance)
        {
            this.instance = instance;
            RefreshView();
        }

        private void RefreshView()
        {
            row.Clear();

            if (allowEditing && instance != null)
            {
                row.Add(CreateEditButton(instance, instance.GetType()));
            }

            foreach (var field in GetOrderedFields(staticDataType))
            {
                row.Add(
                    InstancesTable.CreateInstanceColumn(
                        GetLabelForField(field, instance)
                    )
                );
            }

            if (HasValidationErrors(instance, validationErrors))
            {
                style.backgroundColor = new Color(255, 0, 0, 0.5f); // dark red
            }
        }

        private static bool HasValidationErrors(StaticData instance, Dictionary<StaticData, List<string>> validationErrors)
        {
            return validationErrors != null
                && instance != null
                && validationErrors.TryGetValue(instance, out var errors)
                && !errors.IsNullOrEmpty();
        }

        /// <summary>
        /// Orders the fields of a static data so the <see cref="StaticData.Name"/> property is displays first.
        /// </summary>
        /// <param name="staticDataType"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetOrderedFields(Type staticDataType)
        {
            var fields = Utils.GetFields(staticDataType);
            Utils.SortFields(fields, staticDataType);
            return fields;
        }

        private string GetLabelForField(FieldInfo fieldInfo, StaticData instance)
        {
            if (instance == null)
            {
                return "null";
            }

            if (typeof(StaticData).IsAssignableFrom(fieldInfo.FieldType))
            {
                return (fieldInfo.GetValue(instance) as StaticData)?.Name;
            }

            if (typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType))
            {
                return (fieldInfo.GetValue(instance) as IEnumerable).ToCommaSeparatedString();
            }

            return $"{fieldInfo.GetValue(instance)}";
        }

        private static ButtonIcon CreateEditButton(StaticData instance, Type staticDataType)
        {
            return new ButtonIcon(() => EditorWindow.InstanceEditorWindow.Open(instance, staticDataType), IconPaths.Edit);
        }

        public void UnBindItem()
        {
            row.Clear();
        }
    }
}