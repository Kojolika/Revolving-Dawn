using System;
using System.Collections.Generic;
using System.Reflection;
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

            row.Add(InstancesView.CreateInstanceColumn(index.ToString()));

            foreach (var field in staticDataType.GetFields(EditorWindow.BindingFlagsToSelectStaticDataFields))
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