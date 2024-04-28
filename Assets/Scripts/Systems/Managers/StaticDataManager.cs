using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using Tooling.Logging;
using UnityEditor;
using UnityEngine;
using Utils;
using Utils.Attributes;
using Utils.Extensions;
using Object = UnityEngine.Object;

namespace Systems.Managers
{
    public class StaticDataManager : IManager
    {
        private static readonly string ScriptableObjectsAssetPath = "Assets/ScriptableObjects";

        public Dictionary<Type, Dictionary<string, ScriptableObject>> AssetDictionary { get; private set; } = new();

        public UniTask Startup()
        {
            AssetDictionary = CreateAssetDictionary();

            return UniTask.CompletedTask;
        }

        public bool TryGetAssetForType<T>(string guid, out T asset) where T : ScriptableObject
        {
            asset = default;
            if (AssetDictionary.IsNullOrEmpty())
            {
                MyLogger.LogError($" Attempting to get asset from asset dictionary when the dictionary hasn't been built yet!");
                return false;
            }

            if (AssetDictionary.ContainsKey(typeof(T)) && AssetDictionary[typeof(T)].TryGetValue(guid, out ScriptableObject value))
            {
                asset = value as T;
                return true;
            }

            return false;
        }

        public List<T> GetAllAssetsForType<T>() where T: ScriptableObject
            => AssetDictionary[typeof(T)].Values.Select(data => data as T).ToList();

        /// <summary>
        /// Iterate through the folder at <see cref="ScriptableObjectsAssetPath"/> and instantiate a copy of each
        /// <see cref="ScriptableObject"/> and store the reference here. We are using this as a small database to get the
        /// static data of cards, classes, mana, items, etc.
        /// </summary>
        /// <remarks>
        private Dictionary<Type, Dictionary<string, ScriptableObject>> CreateAssetDictionary()
        {
            MyLogger.Log($"Generating asset dictionary in {ScriptableObjectsAssetPath}");

            Dictionary<Type, Dictionary<string, ScriptableObject>> Assets = new();

            var ids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}", new[] { "Assets/ScriptableObjects" });
            foreach (var id in ids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(id);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                var assetType = asset.GetType();

                // Create a new instance in case the value of the scriptableObject changes at runtime
                var newAssetInstance = Object.Instantiate(asset);

                string assetId = $"{assetPath}/{asset.name}";

                if (Assets.ContainsKey(assetType))
                {
                    Assets[assetType].Add(assetId, newAssetInstance);
                }
                else
                {
                    Assets.Add(
                        assetType,
                        new Dictionary<string, ScriptableObject>()
                        {
                            { assetId, newAssetInstance }
                        }
                    );
                }
            }

            return Assets;
        }
    }
}