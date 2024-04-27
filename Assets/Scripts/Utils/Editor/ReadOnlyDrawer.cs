using Tooling.Logging;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace Data.Utils.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnly<>))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var value = property.FindPropertyRelative("value");
            var propertyField = new PropertyField(value);
            propertyField.BindProperty(value);
            propertyField.label = $"[ReadOnly] {property.displayName}";
            propertyField.style.backgroundColor = Color.grey;
            root.Add(propertyField);
            return root;
        }
    }
}