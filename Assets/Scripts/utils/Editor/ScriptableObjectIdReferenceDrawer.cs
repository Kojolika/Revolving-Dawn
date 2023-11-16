#if UNITY_EDITOR
using System;
using System.Linq;
using Systems.Managers;
using Tooling.Logging;
using UnityEditor;
using UnityEngine;
using Utils.Attributes;

namespace Scripts.Utils.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableObjectIdReferenceAttribute))]
    public class ScriptableObjectIdReferenceDrawer : PropertyDrawer
    {
        readonly StaticDataManager staticDataManager = new();
        int currentIndex = 0;
        int currentIndexForId = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var assets = staticDataManager.CreateAssetDictionary();
            var assetTypes = assets.Keys.Select(type => type.Name).ToArray();
            var stringIds = assets.Values.SelectMany(value => value.Keys).ToArray();

            EditorGUILayout.BeginVertical();

            EditorGUI.BeginChangeCheck();

            currentIndex = EditorGUILayout.Popup("Asset Type:", currentIndex, assetTypes);
            var currentType = assets.Keys.ToList()[currentIndex];
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

            currentIndexForId = EditorGUILayout.Popup(
                $"{currentType.Name} ID:",
                currentIndexForId,
                currentOptionsValues
            );

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = currentOptionsKeys[currentIndexForId];

                var objectWithProp = property.serializedObject.targetObject;

                Undo.RecordObject(objectWithProp, fieldInfo.GetValue(objectWithProp).ToString());
            }

            // Don't allow the value to be edited
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }
    }
}
#endif