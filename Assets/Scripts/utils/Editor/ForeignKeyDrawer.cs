#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Systems.Managers;
using UnityEditor;
using UnityEngine;
using Utils;
using Utils.Attributes;

namespace Scripts.Utils.Editor
{
    [CustomPropertyDrawer(typeof(ForeignKeyAttribute))]
    public class ForeignKeyDrawer : PropertyDrawer
    {
        readonly StaticDataManager staticDataManager = new();
        int guidIndex = 0;

        bool showPrimaryKeySelection = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var assetDictionary = staticDataManager.CreateAssetDictionary();

            var objectWithProp = property.serializedObject.targetObject;
            var fieldValue = fieldInfo.GetValue(objectWithProp);
            var typeOfClassWithForeignKey = (attribute as ForeignKeyAttribute).type;

            var currentGuid = fieldValue is ReadOnly<string> readOnlyString
                ? readOnlyString
                : (string)fieldValue;

            guidIndex = GetGuidIndex(assetDictionary, currentGuid);

            showPrimaryKeySelection = EditorGUILayout.BeginFoldoutHeaderGroup(showPrimaryKeySelection, $"[Foreign Key] {label}:");

            if (showPrimaryKeySelection)
            {
                EditorGUILayout.HelpBox($"Select an asset of type {typeOfClassWithForeignKey}", MessageType.Info);

                // Don't allow the value to be edited, it's value is changed by the above Popups
                GUI.enabled = false;
                EditorGUILayout.PropertyField(property, new GUIContent("Foreign Key: "));
                GUI.enabled = true;

                EditorGUI.BeginChangeCheck();

                var currentOptionsKeys = assetDictionary[typeOfClassWithForeignKey].Keys.ToArray();
                var currentOptionsValues = assetDictionary[typeOfClassWithForeignKey].Values
                    .Select(obj => obj.name)
                    .Select(name =>
                    {
                        var substring = "(Clone)";
                        if (name.EndsWith(substring))
                        {
                            name = name[..name.LastIndexOf(substring)];
                        }
                        return name;
                    })
                    .ToArray();

                guidIndex = EditorGUILayout.Popup(
                    $"{typeOfClassWithForeignKey.Name} ID:",
                    guidIndex,
                    currentOptionsValues
                );

                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = currentOptionsKeys[guidIndex];
                    fieldInfo.SetValue(objectWithProp, currentOptionsKeys[guidIndex]);

                    var serializedObject = property.serializedObject;

                    // Unity docs reccomend SetDirty and RecordObject for scriptableObjects
                    EditorUtility.SetDirty(objectWithProp);
                    Undo.RecordObject(objectWithProp, fieldInfo.GetValue(objectWithProp).ToString());

                    // Not sure if neccesary?
                    serializedObject.Update();
                    serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }


        int GetGuidIndex(Dictionary<System.Type, Dictionary<string, ScriptableObject>> assetDictionary, string currentGuid)
        {
            int guidIndex = -1;

            var guidsForTypeArray = assetDictionary[(attribute as ForeignKeyAttribute).type].ToArray();
            for (int qndex = 0; qndex < guidsForTypeArray.Length; qndex++)
            {
                var kvp = guidsForTypeArray[qndex];
                if (kvp.Key == currentGuid)
                {
                    guidIndex = qndex;

                    break;
                }
            }


            return guidIndex;
        }
    }

}
#endif