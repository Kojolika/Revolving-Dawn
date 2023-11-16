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
    public class ScriptableObjectIdReferenceAttribute : PropertyDrawer
    {
        StaticDataManager staticDataManager = new();
        int currentIndex = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var assets = staticDataManager.CreateAssetDictionary();

            var stringIds = assets.Values.SelectMany(value => value.Keys).ToArray();

            foreach(var id in stringIds){
                MyLogger.Log($"ID: {id}");
            }

            currentIndex = EditorGUILayout.Popup(currentIndex, stringIds);
        }
    }
}
#endif