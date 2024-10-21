using System;
using System.Linq;
using System.Text.RegularExpressions;
using ModestTree;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Attributes;
using UnityEngine.Serialization;
using Tooling.Logging;


[CustomPropertyDrawer(typeof(DisplayAbstractAttribute))]
public class DisplayAbstractDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();
        var abstractType = (attribute as DisplayAbstractAttribute).Type;
        var derivedTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsAbstract
                && !type.IsInterface
                && abstractType.IsAssignableFrom(type)
                && type.HasAttribute<SerializableAttribute>())
            .ToArray();

        if (derivedTypes.Length == 0)
        {
            root.Add(new Label($"No derived types of type {abstractType} can be displayed. They must be concrete types and have the System.SerializableAttribute to be shown in the inspector"));
            return root;
        }

        var defaultDisplayType = derivedTypes.FirstOrDefault();
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

        var dropdownLabel = $"Select an {abstractType.Name}";
        var dropdown = new DropdownField(dropdownLabel, derivedTypes.Select(type => type.Name).ToList(), defaultSelection);
        var propertyField = DisplayConcreteType(defaultDisplayType, property);

        dropdown.RegisterCallback((ChangeEvent<string> evt) =>
        {
            var newType = derivedTypes
                .Where(type => type.Name == evt.newValue)
                .FirstOrDefault();

            propertyField = DisplayConcreteType(newType, property);
        });

        StyleDropdown(dropdown);
        root.Add(dropdown);
        root.Add(propertyField);

        return root;
    }

    public PropertyField DisplayConcreteType(Type type, SerializedProperty property)
    {
        if (type == null)
        {
            return null;
        }

        if (property.propertyType == SerializedPropertyType.ManagedReference
            && (property.managedReferenceId == ManagedReferenceUtility.RefIdNull || property.type != $"managedReference<{type.Name}>"))
        {
            property.serializedObject.Update();

            try
            {
                property.managedReferenceValue = Activator.CreateInstance(type);
            }
            catch (MissingMemberException)
            {
                MyLogger.LogError($"{type} must have a constructor without parameters to be selected in this dropdown!");
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        var propertyField = new PropertyField(property);
        propertyField.BindProperty(property);
        return propertyField;
    }

    void StyleDropdown(VisualElement dropdown)
    {
        dropdown.style.borderBottomColor = Color.white;
        dropdown.style.borderBottomWidth = 1;
        dropdown.style.marginBottom = 5;
    }
}