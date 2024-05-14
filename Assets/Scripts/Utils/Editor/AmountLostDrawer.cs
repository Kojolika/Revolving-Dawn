using System;
using Models;
using Tooling.Logging;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Models.Buffs;

[CustomPropertyDrawer(typeof(AmountLost<>))]
public class AmountLostDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        var label = new Label("Amount Lost:");
        label.style.borderBottomColor = Color.white;
        label.style.borderBottomWidth = 1;
        root.Add(label);

        DisplayProperty(root, property);

        ApplyStyling(root);

        return root;
    }

    void DisplayProperty(VisualElement root, SerializedProperty property)
    {
        var propertyCopy = property.Copy();
        var amountContainer = new VisualElement();

        /// <summary>
        /// Must match the T property of <see cref="AmountLost<>"/> 
        /// </summary>
        SerializedProperty amountProperty = property.FindPropertyRelative("Amount");
        /// <summary>
        /// Must match the <see cref="LostType"/> property of <see cref="AmountLost<>"/> 
        /// </summary>
        SerializedProperty lostTypeProperty = property.FindPropertyRelative("TypeOfLost");

        var enumPropertyField = new PropertyField(lostTypeProperty);
        enumPropertyField.BindProperty(lostTypeProperty);
        root.Add(enumPropertyField);

        var amountPropertyField = new PropertyField(amountProperty);
        amountPropertyField.BindProperty(amountProperty);
        root.Add(amountPropertyField);

        enumPropertyField.RegisterValueChangeCallback((SerializedPropertyChangeEvent evt) =>
        {
            /// <summary>
            /// 0 is <see cref="LostType.Amount"/>
            /// 1 is <see cref="LostType.All"/>
            /// </summary>
            if (evt.changedProperty.enumValueFlag == 0)
            {
                amountPropertyField.SetEnabled(true);
                amountPropertyField.style.visibility = Visibility.Visible;
            }
            else
            {
                amountPropertyField.SetEnabled(false);
                amountPropertyField.style.visibility = Visibility.Hidden;
            }
        });
    }

    void ApplyStyling(VisualElement element)
    {
        var padding = 5;
        element.style.paddingBottom = padding;
        element.style.paddingLeft = padding;
        element.style.paddingRight = padding;
        element.style.paddingTop = padding;

        var borderWidth = 0.5f;
        element.style.borderBottomWidth = borderWidth;
        element.style.borderLeftWidth = borderWidth;
        element.style.borderRightWidth = borderWidth;
        element.style.borderTopWidth = borderWidth;

        var borderColor = Color.green;
        element.style.borderBottomColor = borderColor;
        element.style.borderLeftColor = borderColor;
        element.style.borderRightColor = borderColor;
        element.style.borderTopColor = borderColor;

        var borderRadius = 5;
        element.style.borderBottomLeftRadius = borderRadius;
        element.style.borderBottomRightRadius = borderRadius;
        element.style.borderTopLeftRadius = borderRadius;
        element.style.borderTopRightRadius = borderRadius;
    }
}