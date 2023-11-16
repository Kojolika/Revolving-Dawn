#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Utils.Attributes;

namespace Scripts.Utils.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableObjectIdReferenceAttribute))]
    public class ScriptableObjectIdReferenceAttribute : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            if (string.IsNullOrEmpty(property.stringValue))
            {
                property.stringValue = Guid.NewGuid().ToString();
            }

            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
#endif