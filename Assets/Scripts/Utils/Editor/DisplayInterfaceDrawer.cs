using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ModestTree;
using Tooling.Logging;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Attributes;

namespace Data.Utils.Editor
{
    [CustomPropertyDrawer(typeof(DisplayInterfaceAttribute))]
    public class DisplayInterfaceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var interfaceType = (attribute as DisplayInterfaceAttribute).Type;
            var derivedTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract
                    && !type.IsInterface
                    && interfaceType.IsAssignableFrom(type)
                    && type.HasAttribute<SerializableAttribute>())
                .ToArray();

            if (derivedTypes.Length == 0)
            {
                root.Add(new Label($"No derived types of type {interfaceType} can be displayed. They must be concrete types and have the System.SerializableAttribute to be shown in the inspector"));
                return root;
            }

            var defaultDisplayType = derivedTypes.FirstOrDefault();
            var label = $"Select an {interfaceType.Name}";
            var regex = new Regex(@"(?<=managedReference<)[^>]+");
            var currentType = property.type;

            string defaultSelection;
            if (regex.IsMatch(currentType))
            {
                defaultSelection = regex.Match(currentType).ToString();
                defaultDisplayType = derivedTypes.Where(type => type.Name == defaultSelection).First();
            }
            else
            {
                defaultSelection = defaultDisplayType.Name;
            }

            MyLogger.Log($"defualt value : {defaultSelection}. current type :{currentType}");
            var dropdown = new DropdownField(label, derivedTypes.Select(type => type.Name).ToList(), defaultSelection);
            root.Add(dropdown);

            var typeDisplay = new VisualElement();
            StyleElement(typeDisplay);
            DisplayConcreteType(defaultDisplayType, root, typeDisplay, property);

            dropdown.RegisterValueChangedCallback((ChangeEvent<string> evt) =>
            {
                var newType = derivedTypes
                    .Where(type => type.Name == evt.newValue)
                    .FirstOrDefault();

                DisplayConcreteType(newType, root, typeDisplay, property);
            });

            return root;
        }

        public void DisplayConcreteType(Type type, VisualElement root, VisualElement displayContainer, SerializedProperty property)
        {

            if (root.Contains(displayContainer))
            {
                displayContainer.Clear();
            }

            if (type == null)
            {
                return;
            }

            var serializedProperty = property.Copy();
            var propName = serializedProperty.name;

            if (serializedProperty.propertyType == SerializedPropertyType.ManagedReference
                && serializedProperty.name == propName
                && serializedProperty.type != $"managedReference<{type.Name}>")
            {
                MyLogger.Log($"Setting obj value, old type {serializedProperty.type}, new type: {type}");
                serializedProperty.serializedObject.Update();
                serializedProperty.managedReferenceValue = Activator.CreateInstance(type);
                serializedProperty.serializedObject.ApplyModifiedProperties();
            }

            var endProperty = serializedProperty.GetEndProperty();
            var visitedProperties = new HashSet<uint>();
            do
            {
                // Only display the property once
                if (SerializedProperty.EqualContents(endProperty, serializedProperty))
                {
                    break;
                }

                // Only display the object
                // m_FileID and m_PathID would be displayed for object references but
                // we don't care to include that in the inspector
                if (serializedProperty.propertyPath.Contains($"{propName}.")
                    && !serializedProperty.propertyPath.Contains("m_FileID")
                    && !serializedProperty.propertyPath.Contains($"m_PathID"))
                {
                    MyLogger.Log($"Displaying {serializedProperty.propertyPath}");
                    var propertyField = new PropertyField(serializedProperty);
                    propertyField.BindProperty(serializedProperty);
                    displayContainer.Add(propertyField);
                }
            }
            while (serializedProperty.Next(true));

            root.Add(displayContainer);
        }

        void StyleElement(VisualElement typeDisplay)
        {
            typeDisplay.style.color = Color.black;
            typeDisplay.style.marginLeft = 30;

            var padding = 10;
            typeDisplay.style.paddingBottom = padding;
            typeDisplay.style.paddingLeft = padding;
            typeDisplay.style.paddingRight = padding;
            typeDisplay.style.paddingTop = padding;

            var borderWidth = 0.5f;
            typeDisplay.style.borderBottomWidth = borderWidth;
            typeDisplay.style.borderLeftWidth = borderWidth;
            typeDisplay.style.borderRightWidth = borderWidth;
            typeDisplay.style.borderTopWidth = borderWidth;

            var borderColor = Color.white;
            typeDisplay.style.borderBottomColor = borderColor;
            typeDisplay.style.borderLeftColor = borderColor;
            typeDisplay.style.borderRightColor = borderColor;
            typeDisplay.style.borderTopColor = borderColor;

            var borderRadius = 5;
            typeDisplay.style.borderBottomLeftRadius = borderRadius;
            typeDisplay.style.borderBottomRightRadius = borderRadius;
            typeDisplay.style.borderTopLeftRadius = borderRadius;
            typeDisplay.style.borderTopRightRadius = borderRadius;
        }
    }
}