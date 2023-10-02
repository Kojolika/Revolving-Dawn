using UnityEngine;
using UnityEditor;
using Utils;

/// For more information on custom property drawers: https://docs.unity3d.com/Manual/editor-PropertyDrawers.html

[CustomPropertyDrawer(typeof(SerializableDictionary<string, float>))]
public class SerializableDictionaryStringFloatPropertyDrawer : PropertyDrawer
{
    bool isFoldedOut = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty keys = property.FindPropertyRelative("keys");
        SerializedProperty values = property.FindPropertyRelative("values");

        int dictSize = Mathf.Min(keys.arraySize, values.arraySize);
        
        isFoldedOut = EditorGUILayout.Foldout(isFoldedOut, "Dictionary");
        

        if (isFoldedOut)
        {
            var level = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField(label: property.displayName);

            EditorGUILayout.HelpBox("List Of Values handles the contents of the dictionary, the  dictionary will update in the OnEnable event", MessageType.Info);

            
            for (int index = 0; index < dictSize; index++)
            {
                EditorGUILayout.LabelField(label: "Element " + index);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(keys.GetArrayElementAtIndex(index));
                EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(index));
                EditorGUI.indentLevel--;
            }


            EditorGUI.indentLevel = level;
        }
        else
        {
            isFoldedOut = false;
        }

        EditorGUI.EndProperty();
    }
}