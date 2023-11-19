#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Utils.Attributes;
using Utils;

namespace Scripts.Utils.Editor
{
    [CustomPropertyDrawer(typeof(PrimaryKeyAttribute))]
    public class PrimaryKeyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;

            string stringValue;

            // Get the object that this property is attached to
            var objectWithID = property.serializedObject.targetObject;

            // If our field is a ReadOnly<string> we'll use reflection to grab the private value of the string
            if (fieldInfo.GetValue(objectWithID) is ReadOnly<string> readOnlyString)
            {
                stringValue = readOnlyString;

                if (string.IsNullOrEmpty(stringValue))
                {
                    stringValue = Guid.NewGuid().ToString();

                    // Get the field on the ReadOnly<string> thats on the objectWithID
                    var readOnlyFieldInfo = readOnlyString
                        .GetType()
                        .GetField("value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    //Set it's private value
                    readOnlyFieldInfo.SetValue(readOnlyString, stringValue);
                }
            }
            else
            {
                stringValue = property.stringValue;
                // If its not a readonly string, use the property.stringValue instead
                if (string.IsNullOrEmpty(stringValue))
                {
                    stringValue = Guid.NewGuid().ToString();
                    property.stringValue = stringValue;
                }
            }

            property.serializedObject.Update();

            //EditorGUILayout.PropertyField(property);
            EditorGUI.TextField(position, label, stringValue);

            GUI.enabled = true;
        }
    }
}
#endif