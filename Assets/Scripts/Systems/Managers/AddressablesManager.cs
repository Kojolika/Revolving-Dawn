using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using Tooling.Logging;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Utils.Extensions;

namespace Systems.Managers
{
    public class AddressablesManager : IManager
    {
        private Dictionary<string, Dictionary<Type, UnityEngine.Object>> loadedAssets = new();
        public async UniTask LoadAssetsFromLabels(
            List<AssetLabelReference> assetLabelReferences,
            Func<bool> releaseCondition,
            Action<UnityEngine.Object> onSuccess = null,
            CancellationToken cancellationToken = default,
            Action onFail = null)
        {
            var resourceLocationsHandle = Addressables.LoadResourceLocationsAsync(assetLabelReferences, Addressables.MergeMode.Union);
            await resourceLocationsHandle.Task;

            int resourceLocationCount = resourceLocationsHandle.Result.Count;
            var loadTasks = new UniTask[resourceLocationCount];
            for (int i = 0; i < resourceLocationCount; i++)
            {
                var resourceLocation = resourceLocationsHandle.Result[i];
                loadTasks[i] = LoadAsset(resourceLocation, releaseCondition, onSuccess, cancellationToken);
            }
            await UniTask.WhenAll(loadTasks).SuppressCancellationThrow();

            Addressables.Release(resourceLocationsHandle);
        }
        private async UniTask LoadAsset(
            IResourceLocation resourceLocation,
            Func<bool> releaseCondition,
            Action<UnityEngine.Object> onSuccess,
            CancellationToken cancellationToken,
            Action onFail = null)
        {
            var key = resourceLocation.PrimaryKey;
            var assetType = resourceLocation.ResourceType;

            if (loadedAssets.TryGetValue(key, out var assetDictionary) && assetDictionary.TryGetValue(assetType, out var asset))
            {
                onSuccess?.Invoke(asset);
            }
            else
            {
                var opHandle = Addressables.LoadAssetAsync<UnityEngine.Object>(resourceLocation);
                try
                {
                    await opHandle.Task;

                    if (!loadedAssets.ContainsKey(key))
                    {
                        loadedAssets[key] = new();
                    }
                    loadedAssets[key][assetType] = opHandle.Result;

                    onSuccess?.Invoke(opHandle.Result);

                    _ = ReleaseWhen(releaseCondition, opHandle, key, cancellationToken);
                }
                catch (Exception e)
                {
                    MyLogger.LogError($"Failed to load addressable {key} of type {assetType} with exception {e.Message}");

                    onFail?.Invoke();
                }

            }
        }
        /// <summary>
        /// Load an asset defined in the addressable groups.
        /// </summary>
        /// <param name="assetReference">Addressable key</param>
        /// <param name="onSuccess">Action when the loading is completed.</param>
        /// <param name="onFail">Action when the loading fails.</param>
        /// <param name="releaseCondition">Pass in a function that will release the asset when the condition is true. </param>
        /// <typeparam name="T">Type of asset to load.</typeparam>
        /// <returns>The loaded asset.</returns>
        public async UniTask<T> LoadGenericAsset<T>(
            AssetReferenceT<T> assetReference,
            Func<bool> releaseCondition,
            Action<T> onSuccess = null,
            Action onFail = null,
            CancellationToken cancellationToken = default
        ) where T : UnityEngine.Object
        {
            var locationsOpHandle = Addressables.LoadResourceLocationsAsync(assetReference, typeof(T));
            var locations = await locationsOpHandle.Task;
            if (locations.IsNullOrEmpty())
            {
                onFail?.Invoke();
                MyLogger.LogError($"Attempted to load asset {assetReference} of type {typeof(T)} but could not find a resource location for the asset.");
                Addressables.Release(locationsOpHandle);
                return null;
            }
            var asset = await LoadGenericAsset(locations[0], releaseCondition, onSuccess, onFail, cancellationToken);
            Addressables.Release(locationsOpHandle);
            return asset;
        }

        public T LoadGenericAssetSync<T>(AssetReferenceT<T> assetReference, Func<bool> releaseCondition, CancellationToken cancellationToken = default)
            where T : UnityEngine.Object
        {
            var locationsOpHandle = Addressables.LoadResourceLocationsAsync(assetReference, typeof(T));
            locationsOpHandle.WaitForCompletion();

            var locations = locationsOpHandle.Result;
            if (locations.IsNullOrEmpty())
            {
                MyLogger.LogError($"Attempted to load asset {assetReference} of type {typeof(T)} but could not find a resource location for the asset.");
                Addressables.Release(locationsOpHandle);
                return null;
            }
            return LoadAssetInternalSync<T>(locations[0].PrimaryKey, releaseCondition, cancellationToken);
        }
        public T LoadGenericAssetSync<T>(string key, Func<bool> releaseCondition, CancellationToken cancellationToken = default)
            where T : UnityEngine.Object
        {
            var locationsOpHandle = Addressables.LoadResourceLocationsAsync(key, typeof(T));
            locationsOpHandle.WaitForCompletion();

            var locations = locationsOpHandle.Result;
            if (locations.IsNullOrEmpty())
            {
                MyLogger.LogError($"Attempted to load asset {key} of type {typeof(T)} but could not find a resource location for the asset.");
                Addressables.Release(locationsOpHandle);
                return null;
            }
            return LoadAssetInternalSync<T>(locations[0].PrimaryKey, releaseCondition, cancellationToken);
        }

        public async UniTask<T> LoadGenericAsset<T>(
            IResourceLocation resourceLocation,
            Func<bool> releaseCondition,
            Action<T> onSuccess = null,
            Action onFail = null,
            CancellationToken cancellationToken = default
        ) where T : UnityEngine.Object
            => await LoadAssetInternal(resourceLocation.PrimaryKey, releaseCondition, onSuccess, onFail, cancellationToken);

        private T LoadAssetInternalSync<T>(
            string key,
            Func<bool> releaseCondition,
            CancellationToken cancellationToken = default
        ) where T : UnityEngine.Object
        {
            if (loadedAssets.TryGetValue(key, out var assetDictionary) && assetDictionary.TryGetValue(typeof(T), out var asset))
            {
                return (T)asset;
            }

            var opHandle = Addressables.LoadAssetAsync<T>(key);
            return HandleAsyncOperationInternalSync<T>(opHandle, key, releaseCondition, cancellationToken);
        }
        private async UniTask<T> LoadAssetInternal<T>(
            string key,
            Func<bool> releaseCondition,
            Action<T> onSuccess = null,
            Action onFail = null,
            CancellationToken cancellationToken = default
        ) where T : UnityEngine.Object
        {
            if (loadedAssets.TryGetValue(key, out var assetDictionary) && assetDictionary.TryGetValue(typeof(T), out var asset))
            {
                onSuccess?.Invoke((T)asset);
                return (T)asset;
            }

            var opHandle = Addressables.LoadAssetAsync<T>(key);
            return await HandleAsyncOperationInternal(opHandle, key, releaseCondition, onSuccess, onFail, cancellationToken);
        }

        private T HandleAsyncOperationInternalSync<T>(
            AsyncOperationHandle opHandle,
            string key,
            Func<bool> releaseCondition,
            CancellationToken cancellationToken = default
            ) where T : UnityEngine.Object
        {
            opHandle.WaitForCompletion();

            var asset = (T)opHandle.Result;
            if (asset == null)
            {
                return null;
            }

            if (!loadedAssets.ContainsKey(key))
            {
                loadedAssets[key] = new();
            }

            loadedAssets[key][typeof(T)] = (T)opHandle.Result;

            _ = ReleaseWhen(releaseCondition, opHandle, key, cancellationToken);

            return asset;
        }

        private async UniTask<T> HandleAsyncOperationInternal<T>(
            AsyncOperationHandle opHandle,
            string key,
            Func<bool> releaseCondition,
            Action<T> onSuccess = null,
            Action onFail = null,
            CancellationToken cancellationToken = default
            ) where T : UnityEngine.Object
        {
            try
            {
                await opHandle.Task;

                if (!loadedAssets.ContainsKey(key))
                {
                    loadedAssets[key] = new();
                }
                loadedAssets[key][typeof(T)] = (T)opHandle.Result;

                onSuccess?.Invoke((T)opHandle.Result);

                _ = ReleaseWhen(releaseCondition, opHandle, key, cancellationToken);
            }
            catch (Exception e)
            {
                MyLogger.LogError($"Failed to load addressable {key} of type {typeof(T)} with exception {e.Message}");

                onFail?.Invoke();
            }

            return (T)opHandle.Result;
        }

        async UniTask ReleaseWhen(
            Func<bool> condition,
            AsyncOperationHandle operationHandle,
            string key,
            CancellationToken cancellationToken
        )
        {
            _ = await UniTask.WaitUntil(condition, cancellationToken: cancellationToken).SuppressCancellationThrow();

            if (operationHandle.IsValid())
            {
                Addressables.Release(operationHandle);
            }

            if (loadedAssets.ContainsKey(key))
            {
                loadedAssets.Remove(key);
            }
        }
    }
}