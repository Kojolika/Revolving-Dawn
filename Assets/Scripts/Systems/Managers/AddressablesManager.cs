using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Systems.Managers
{
    public class AddressablesManager : IManager
    {
        async UniTask IManager.Startup()
        {
            MyLogger.Log("Booting up Addressables.");
            var addressableHandle = Addressables.InitializeAsync();
            await UniTask.WaitUntil(() => addressableHandle.IsDone);
        }

        private Dictionary<object, UnityEngine.Object> loadedAssets = new();

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
            var locations = await Addressables.LoadResourceLocationsAsync(assetReference);
            return await LoadGenericAsset(locations[0], releaseCondition, onSuccess, onFail, cancellationToken);
        }

        public async UniTask<T> LoadGenericAsset<T>(
            IResourceLocation resourceLocation,
            Func<bool> releaseCondition,
            Action<T> onSuccess = null,
            Action onFail = null,
            CancellationToken cancellationToken = default
        ) where T : UnityEngine.Object
            => await LoadAssetInternal(resourceLocation.PrimaryKey, releaseCondition, onSuccess, onFail, cancellationToken);

        private async UniTask<T> LoadAssetInternal<T>(
            string key,
            Func<bool> releaseCondition,
            Action<T> onSuccess = null,
            Action onFail = null,
            CancellationToken cancellationToken = default
        ) where T : UnityEngine.Object
        {
            if (loadedAssets.TryGetValue(key, out var asset))
            {
                onSuccess?.Invoke(asset as T);
                return asset as T;
            }

            var opHandle = Addressables.LoadAssetAsync<T>(key);
            return await HandleAsyncOperationInternal(opHandle, key, releaseCondition, onSuccess, onFail, cancellationToken);
        }

        private async UniTask<T> HandleAsyncOperationInternal<T>(AsyncOperationHandle opHandle,
            object key,
            Func<bool> releaseCondition,
            Action<T> onSuccess = null,
            Action onFail = null,
            CancellationToken cancellationToken = default
            ) where T : UnityEngine.Object
        {
            try
            {
                await opHandle.Task;
                loadedAssets[key] = opHandle.Result as T;

                _ = ReleaseWhen(releaseCondition, opHandle, key, cancellationToken);
            }
            catch (Exception e)
            {
                MyLogger.LogError($"Failed to load addressable {key} of type {typeof(T)} with exception {e}");

                onFail?.Invoke();
            }

            onSuccess?.Invoke(opHandle.Result as T);

            return opHandle.Result as T;
        }

        async UniTask ReleaseWhen(
            Func<bool> condition,
            AsyncOperationHandle operationHandle,
            object key,
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