using System;
using Tooling.Logging;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Utils.Editor
{
    [CustomPropertyDrawer(typeof(Settings.NumberSettings<>))]
    public class NumberSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            root.Add(new Label(property.name));

            // string must correspond to NumberSettings<T>.settingsType
            var settingsTypeProp = property.FindPropertyRelative("settingsType");

            var settingsTypePropField = new PropertyField(settingsTypeProp);
            settingsTypePropField.BindProperty(settingsTypeProp);
            root.Add(settingsTypePropField);

            // string must correspond to NumberSettings<T>.value
            var valueProp = property.FindPropertyRelative("value");

            var valuPropField = new PropertyField(valueProp);
            valuPropField.BindProperty(valueProp);
            root.Add(valuPropField);

            settingsTypePropField.RegisterValueChangeCallback((SerializedPropertyChangeEvent evt) =>
            {
                valuPropField.style.visibility = evt.changedProperty.enumValueIndex == 0
                    ? Visibility.Visible
                    : Visibility.Hidden;
            });

            return root;
        }
    }
}