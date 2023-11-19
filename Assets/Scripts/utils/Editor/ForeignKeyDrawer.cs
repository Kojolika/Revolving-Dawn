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
        int typeIndex = 0;
        int guidIndex = 0;

        bool showPrimaryKeySelection = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var assets = staticDataManager.CreateAssetDictionary();
            var availableTypes = assets.Keys.ToArray();
            var objectWithProp = property.serializedObject.targetObject;
            var fieldValue = fieldInfo.GetValue(objectWithProp);

            var currentGuid = fieldValue is ReadOnly<string> readOnlyString
                ? readOnlyString
                : (string)fieldValue;

            (typeIndex, guidIndex) = GetIndexes(assets, availableTypes, currentGuid);

            var typeOptions = availableTypes.Select(type => type.Name).ToArray();

            showPrimaryKeySelection = EditorGUILayout.BeginFoldoutHeaderGroup(showPrimaryKeySelection, $"Primary Key selection for {label}:");

            if (showPrimaryKeySelection)
            {
                EditorGUILayout.HelpBox("Select the type of asset to get a primary key from.", MessageType.Info);

                // Don't allow the value to be edited, it's value is changed by the above Popups
                GUI.enabled = false;
                EditorGUILayout.PropertyField(property, new GUIContent("Primary Key: "));
                GUI.enabled = true;

                EditorGUI.BeginChangeCheck();
                typeIndex = EditorGUILayout.Popup("Asset type:", typeIndex, typeOptions);
                var currentType = assets.Keys.ToList()[typeIndex];
                var currentOptionsKeys = assets[currentType].Keys.ToArray();
                var currentOptionsValues = assets[currentType].Values
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
                    $"{currentType.Name} ID:",
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


        (int, int) GetIndexes(
            Dictionary<System.Type, Dictionary<string, ScriptableObject>> assetDictionary,
            System.Type[] availableTypes,
            string currentGuid
        )
        {
            int typeIndex = default;
            int guidIndex = default;
            // Find the current index by iterating through the asset dictionary to update the values in the inspector
            // otherwise the values would reset
            for (int index = 0; index < availableTypes.Length; index++)
            {
                var type = availableTypes[index];
                if (assetDictionary[type].ContainsKey(currentGuid))
                {
                    typeIndex = index;
                    var guidsForTypeArray = assetDictionary[type].ToArray();
                    for (int qndex = 0; qndex < guidsForTypeArray.Length; qndex++)
                    {
                        var kvp = guidsForTypeArray[qndex];
                        if (kvp.Key == currentGuid)
                        {
                            guidIndex = qndex;

                            break;
                        }
                    }
                    break;
                }
            }

            return (typeIndex, guidIndex);
        }
    }

}
#endif