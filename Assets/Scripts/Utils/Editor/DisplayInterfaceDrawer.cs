using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ModestTree;
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

            var dropdown = new DropdownField(label, derivedTypes.Select(type => type.Name).ToList(), defaultSelection);
            StyleDropdown(dropdown);
            root.Add(dropdown);

            var typeDisplay = new VisualElement();
            StyleInterfaceElement(root);
            DisplayConcreteType(defaultDisplayType, root, typeDisplay, property);

            dropdown.RegisterCallback((ChangeEvent<string> evt) =>
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
                serializedProperty.serializedObject.Update();
                serializedProperty.managedReferenceValue = Activator.CreateInstance(type);
                serializedProperty.serializedObject.ApplyModifiedProperties();
            }

            var endProperty = serializedProperty.GetEndProperty();

            // Start iterating upon the first managedReference
            serializedProperty.Next(true);

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
                    var propertyField = new PropertyField(serializedProperty);
                    propertyField.BindProperty(serializedProperty);
                    displayContainer.Add(propertyField);
                }
            }
            while (serializedProperty.Next(serializedProperty.propertyType != SerializedPropertyType.ManagedReference || !DoesPropertyHaveAttribute(serializedProperty)));

            root.Add(displayContainer);
        }

        /// <summary>
        /// Checks to see a property has a <see cref="DisplayInterfaceAttribute"/>.
        /// </summary>
        /// <param name="property">Property to check.</param>
        /// <returns>True if the property has a <see cref="DisplayInterfaceAttribute"/></returns>
        bool DoesPropertyHaveAttribute(SerializedProperty property)
        {
            var targetObjectType = property.serializedObject.targetObject.GetType();
            string[] objectPropertyNames = property.propertyPath.Split('.');
            for (int i = 0; i < objectPropertyNames.Length; i++)
            {
                string propName = objectPropertyNames[i];

                if (i + 1 < objectPropertyNames.Length && objectPropertyNames[i + 1] == "Array"
                    && i + 2 < objectPropertyNames.Length && objectPropertyNames[i + 2].StartsWith("data["))
                {
                    i += 2;
                }

                var fieldInfo = targetObjectType.GetField(propName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                if (fieldInfo != null)
                {
                    if (fieldInfo.FieldType.IsInterface && fieldInfo.GetCustomAttribute<DisplayInterfaceAttribute>() != null)
                    {
                        return true;
                    }

                    // Check if object is array, get array type
                    targetObjectType = fieldInfo.FieldType.GetElementType() ?? fieldInfo.FieldType;

                    // otherwise if the object is an IEnumerable, find the type of IEnumerable
                    if (typeof(IEnumerable).IsAssignableFrom(targetObjectType))
                    {
                        if (targetObjectType.IsGenericType() && typeof(IEnumerable<>) == targetObjectType.GetGenericTypeDefinition())
                        {
                            targetObjectType = targetObjectType.GetGenericArguments()[0];
                        }
                        else
                        {
                            foreach (var iType in targetObjectType.GetInterfaces())
                            {
                                if (iType.IsGenericType() && typeof(IEnumerable<>) == iType.GetGenericTypeDefinition())
                                {
                                    targetObjectType = iType.GetGenericArguments()[0];
                                }
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        void StyleDropdown(VisualElement dropdown)
        {
            dropdown.style.borderBottomColor = Color.white;
            dropdown.style.borderBottomWidth = 1;
            dropdown.style.marginBottom = 5;
        }

        void StyleInterfaceElement(VisualElement typeDisplay)
        {
            typeDisplay.style.color = Color.black;
            typeDisplay.style.marginLeft = 30;

            var padding = 5;
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