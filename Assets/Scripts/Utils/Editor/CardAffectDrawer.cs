using Models.CardAffects;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Tooling.Logging;

[CustomPropertyDrawer(typeof(CardAffectContainer))]
public class CardAffectDrawer : PropertyDrawer
{
    /// <summary>
    /// This must match the <see cref="ICardAffect"/> field name of the <see cref="CardAffectContainer"/> class.  
    /// </summary>
    const string CardAffectVariableName = "cardAffect";

    
    
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();
        var individualCardAffectValueContainer = new VisualElement();
        var label = new Label("Card Affect");
        var definitionProperty = property.FindPropertyRelative("cardAffectDefinition");
        var cardAffectDefinition = new PropertyField(definitionProperty);
        cardAffectDefinition.RegisterValueChangeCallback((SerializedPropertyChangeEvent evt) =>
        {
            CardAffectDefinition newValue = evt.changedProperty.objectReferenceValue as CardAffectDefinition;
            if(root.Contains(individualCardAffectValueContainer))
            {
                root.Remove(individualCardAffectValueContainer);
            }
            individualCardAffectValueContainer = new VisualElement();
            if (newValue == null)
            {
                return;
            }

            var cardAffectTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract
                    && type.BaseType != null
                    && type.BaseType.IsGenericType
                    && type.BaseType.GetGenericTypeDefinition() == typeof(CardAffect<>)
                    && type.BaseType.GenericTypeArguments.Contains(newValue.GetType()))
                .ToArray();

            if (cardAffectTypes.Length == 0)
            {
                return;
            }

            if (cardAffectTypes.Length > 1)
            {
                throw new Exception("Error: There can only be 1 class per T that derives from CardAffect<T>");
            }

            var cardAffectType = cardAffectTypes[0];
            var cardAffectInstance = Activator.CreateInstance(cardAffectType);
            var serializedProperty = property.FindPropertyRelative(CardAffectVariableName);
            do
            {
                Debug.Log($"\tFound {serializedProperty.propertyPath} (type {serializedProperty.propertyType})" +
                    $"(depth {serializedProperty.depth})");

                if (serializedProperty.propertyType == SerializedPropertyType.ManagedReference
                    && serializedProperty.name == CardAffectVariableName
                    && serializedProperty.type != $"managedReference<{cardAffectType.Name}>")
                {
                    serializedProperty.serializedObject.Update();
                    serializedProperty.managedReferenceValue = cardAffectInstance;
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                }

                if (serializedProperty.propertyPath.Contains($".{CardAffectVariableName}."))
                {
                    var propertyField = new PropertyField(serializedProperty);
                    propertyField.BindProperty(serializedProperty);
                    individualCardAffectValueContainer.Add(propertyField);
                }
            }
            while (serializedProperty.Next(true));

            root.Add(individualCardAffectValueContainer);
        });

        root.Add(label);
        root.Add(cardAffectDefinition);

        return root;
    }
}