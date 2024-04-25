using Models.CardEffects;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using UnityEngine;

[CustomPropertyDrawer(typeof(CardEffectContainer))]
public class CardEffectDrawer : PropertyDrawer
{
    /// <summary>
    /// This must match the <see cref="ICardEffect"/> field name of the <see cref="CardEffectContainer"/> class.  
    /// </summary>
    const string CardEffectVariableName = "cardEffect";

    /// <summary>
    /// This must match the <see cref="CardEffectDefinition"/> field name of the <see cref="CardEffectContainer"/> class.  
    /// </summary>
    const string CardEffectDefinitionVariableName = "cardEffectDefinition";

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();
        var individualCardEffectValueContainer = new VisualElement();
        var label = new Label("Card Effect");
        var definitionProperty = property.FindPropertyRelative(CardEffectDefinitionVariableName);
        var cardEffectDefinition = new PropertyField(definitionProperty);

        cardEffectDefinition.RegisterValueChangeCallback((SerializedPropertyChangeEvent evt) =>
        {
            CardEffectDefinition newValue = evt.changedProperty.objectReferenceValue as CardEffectDefinition;

            if (root.Contains(individualCardEffectValueContainer))
            {
                root.Remove(individualCardEffectValueContainer);
            }
            individualCardEffectValueContainer = new VisualElement();

            if (newValue == null)
            {
                return;
            }

            // The intent here is we want to get the specific derived type which corresponds
            // to the CardEffectDefinition. We note that we will only have one
            // derived type of CardEffect for each CardEffectDefinition which allows us this flexibility
            var cardEffectTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract
                    && type.BaseType != null
                    && type.BaseType.IsGenericType
                    && type.BaseType.GetGenericTypeDefinition() == typeof(CardEffect<>)
                    && type.BaseType.GenericTypeArguments.Contains(newValue.GetType()))
                .ToArray();

            if (cardEffectTypes.Length == 0)
            {
                return;
            }

            if (cardEffectTypes.Length > 1)
            {
                throw new Exception("Error: There can only be 1 class per T that derives from CardEffect<T>");
            }

            var cardEffectType = cardEffectTypes[0];
            var serializedProperty = property.FindPropertyRelative(CardEffectVariableName);
            var endProperty = serializedProperty.GetEndProperty();
            do
            {
                // Once we find the end of the cardEffect property we do not need to display anything else
                if (SerializedProperty.EqualContents(endProperty, serializedProperty))
                {
                    break;
                }

                if (serializedProperty.propertyType == SerializedPropertyType.ManagedReference
                    && serializedProperty.name == CardEffectVariableName
                    && serializedProperty.type != $"managedReference<{cardEffectType.Name}>")
                {
                    serializedProperty.serializedObject.Update();
                    serializedProperty.managedReferenceValue = Activator.CreateInstance(cardEffectType);
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                }

                // Only display the cardEffect object
                // m_FileID and m_PathID would be displayed for object references but
                // we don't care to include that in the inspector
                if (serializedProperty.propertyPath.Contains($".{CardEffectVariableName}.")
                    && !serializedProperty.propertyPath.Contains("m_FileID")
                    && !serializedProperty.propertyPath.Contains($"m_PathID"))
                {
                    var propertyField = new PropertyField(serializedProperty);
                    propertyField.BindProperty(serializedProperty);
                    individualCardEffectValueContainer.Add(propertyField);
                }
            }
            while (serializedProperty.Next(true));

            root.Add(individualCardEffectValueContainer);
        });

        root.Add(label);
        root.Add(cardEffectDefinition);

        return root;
    }
}