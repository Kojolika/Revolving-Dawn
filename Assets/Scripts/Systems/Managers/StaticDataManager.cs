using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using Tooling.Logging;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.Managers
{
    public class StaticDataManager : IManager
    {
        private static readonly string ScriptableObjectsAssetPath = "Assets/ScriptableObjects";

        public Dictionary<System.Type, Dictionary<string, ScriptableObject>> Assets { get; private set; } = new();

        /// <summary>
        /// Iterate through the folder at <see cref="ScriptableObjectsAssetPath"/> and instantiate a copy of each
        /// <see cref="ScriptableObject"/> and store the reference here. We are using this as a small database to get the
        /// static data of cards, classes, items, etc.
        /// </summary>
        public UniTask Startup()
        {
            MyLogger.Log($"Searching in {ScriptableObjectsAssetPath}");

            var ids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}", new[] { "Assets/ScriptableObjects" });
            foreach (var id in ids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(id);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                var assetType = asset.GetType();

                // Create a new instance in case the value of the scriptableObject changes at runtime
                var newAssetInstance = Object.Instantiate(asset);
                
                //TODO: use reflection to find ScriptableObjectIdAttribute
                // make this into its own function so we can get this value for the ScriptableObjectIdReferenceAttribute

                MyLogger.Log($"Adding new asset {newAssetInstance} for type {assetType.Name} with guid {id}");
                if (Assets.ContainsKey(assetType))
                {
                    Assets[assetType].Add(id, newAssetInstance);
                }
                else
                {
                    Assets.Add(
                        assetType,
                        new Dictionary<string, ScriptableObject>()
                        {
                            { id, newAssetInstance }
                        }
                    );
                }
            }

            return UniTask.CompletedTask;
        }
    }
}