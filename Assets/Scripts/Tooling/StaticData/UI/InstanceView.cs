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

        public void UnBindItem()
        {
            row.Clear();
        }
    }
}