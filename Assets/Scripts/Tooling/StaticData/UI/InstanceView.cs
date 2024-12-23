using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tooling.StaticData
{
    public class InstanceView : VisualElement
    {
        public StaticData Instance { get; private set; }

        private readonly VisualElement row;
        private readonly Type staticDataType;
        private readonly bool allowEditing;

        public const float EditButtonWidth = 40;

        public InstanceView(Type staticDataType, bool allowEditing)
        {
            this.staticDataType = staticDataType;
            this.allowEditing = allowEditing;

            row = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row }
            };

            Add(row);
        }

        public void BindItem(int index, StaticData instance, List<string> validationErrors)
        {
            Instance = instance;

            row.Clear();

            if (allowEditing)
            {
                row.Add(CreateEditButton(instance, instance.GetType()));
            }

            foreach (var field in GetOrderedFields(staticDataType))
            {
                row.Add(
                    InstancesView.CreateInstanceColumn(
                        GetLabelForField(field, instance)
                    )
                );
            }

            if (validationErrors?.Count > 0)
            {
                style.backgroundColor = new Color(255, 0, 0, 0.5f); // dark red
            }
        }

        /// <summary>
        /// Orders the fields of a static data so the <see cref="StaticData.Name"/> property is displays first.
        /// </summary>
        /// <param name="staticDataType"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetOrderedFields(Type staticDataType)
        {
            var fields = staticDataType.GetFields(EditorWindow.BindingFlagsToSelectStaticDataFields);
            int nameIndex = 0;

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType != typeof(string) || fields[i].Name != "Name")
                {
                    continue;
                }

                nameIndex = i;
            }

            (fields[0], fields[nameIndex]) = (fields[nameIndex], fields[0]);

            return fields;
        }

        private string GetLabelForField(FieldInfo fieldInfo, StaticData instance)
        {
            if (typeof(StaticData).IsAssignableFrom(fieldInfo.FieldType))
            {
                return (fieldInfo.GetValue(instance) as StaticData)?.Name;
            }

            return $"{fieldInfo.GetValue(instance)}";
        }

        private Button CreateEditButton(StaticData instance, Type staticDataType)
        {
            return new Button(() =>
            {
                var instanceEditor = UnityEditor.EditorWindow.GetWindow<EditorWindow.InstanceEditorWindow>();

                instanceEditor.Initialize(instance, staticDataType);
            })
            {
                text = "Edit",
                style = { width = EditButtonWidth }
            };
        }

        public void UnBindItem()
        {
            Instance = null;
            row.Clear();
        }
    }
}