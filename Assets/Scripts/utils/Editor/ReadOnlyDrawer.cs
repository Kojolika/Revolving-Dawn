using Tooling.Logging;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Data.Utils.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnly<>))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
            EditorGUILayout.PropertyField(property, label);
        }
    }
}