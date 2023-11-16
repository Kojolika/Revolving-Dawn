using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using Tooling.Logging;
using UnityEditor;
using UnityEngine;
using Utils.Attributes;
using Object = UnityEngine.Object;

namespace Systems.Managers
{
    public class StaticDataManager : IManager
    {
        private static readonly string ScriptableObjectsAssetPath = "Assets/ScriptableObjects";

        public Dictionary<Type, Dictionary<string, ScriptableObject>> Assets { get; private set; } = new();

        /// <summary>
        /// Iterate through the folder at <see cref="ScriptableObjectsAssetPath"/> and instantiate a copy of each
        /// <see cref="ScriptableObject"/> and store the reference here. We are using this as a small database to get the
        /// static data of cards, classes, items, etc.
        /// </summary>
        public UniTask Startup()
        {
            Assets = CreateAssetDictionary();

            return UniTask.CompletedTask;
        }

        public Dictionary<Type, Dictionary<string, ScriptableObject>> CreateAssetDictionary()
        {
            MyLogger.Log($"Searching in {ScriptableObjectsAssetPath}");

            Dictionary<Type, Dictionary<string, ScriptableObject>> Assets = default;

            var ids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}", new[] { "Assets/ScriptableObjects" });
            foreach (var id in ids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(id);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                var assetType = asset.GetType();

                // Create a new instance in case the value of the scriptableObject changes at runtime
                var newAssetInstance = Object.Instantiate(asset);

                string guid = default;

                //TODO: use reflection to find ScriptableObjectIdAttribute
                foreach (var field in assetType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    var attributes = field.GetCustomAttributes(false);
                    foreach (var attribute in attributes)
                    {
                        if(attribute is ScriptableObjectIdAttribute idAttribute){
                            MyLogger.Log("Found ScriptableObjectIdReferenceAttribute");
                            guid = (string)field.GetValue(newAssetInstance);
                        }
                    }
                }

                if(guid == default){
                    MyLogger.LogError($"No id asset found for {newAssetInstance} of type {assetType}");
                    throw new Exception();
                }

                MyLogger.Log($"Adding new asset {newAssetInstance} for type {assetType.Name} with guid {guid}");
                if (Assets.ContainsKey(assetType))
                {
                    Assets[assetType].Add(guid, newAssetInstance);
                }
                else
                {
                    Assets.Add(
                        assetType,
                        new Dictionary<string, ScriptableObject>()
                        {
                            { guid, newAssetInstance }
                        }
                    );
                }
            }

            return Assets;
        }
    }
}