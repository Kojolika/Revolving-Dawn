using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tooling.StaticData
{
    public class InstanceView : VisualElement
    {
        private readonly VisualElement row;
        private readonly Type staticDataType;

        public const float EditButtonWidth = 40;

        public InstanceView(Type staticDataType)
        {
            this.staticDataType = staticDataType;

            row = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row }
            };

            Add(row);
        }

        public void BindItem(int index, StaticData instance, List<string> validationErrors)
        {
            row.Clear();

            row.Add(CreateEditButton(instance, instance.GetType()));
            row.Add(InstancesView.CreateInstanceColumn(index.ToString()));

            foreach (var field in staticDataType.GetFields(EditorWindow.BindingFlagsToSelectStaticDataFields))
            {
                row.Add(InstancesView.CreateInstanceColumn($"{field.GetValue(instance)}"));
            }

            if (validationErrors?.Count > 0)
            {
                style.backgroundColor = new Color(255, 0, 0, 0.5f); // dark red
            }
        }

        private Button CreateEditButton(StaticData instance, Type staticDataType)
        {
            return new Button(() =>
            {
                var instanceEditor = UnityEditor.EditorWindow.GetWindow<EditorWindow.InstanceEditorWindow>();

                instanceEditor.Initialize(instance, staticDataType);

                instanceEditor.Show();
                instanceEditor.Focus();
            })
            {
                text = "Edit",
                style = { width = EditButtonWidth }
            };
        }

        public void UnBindItem()
        {
            row.Clear();
        }
    }
}