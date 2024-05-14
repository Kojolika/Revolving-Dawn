using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
        /// <summary>
        /// Load an asset defined in the addressable groups.
        /// </summary>
        /// <param name="key">Addressable key</param>
        /// <param name="onSuccess">Action when the loading is completed.</param>
        /// <param name="onFail">Action when the loading fails.</param>
        /// <param name="releaseCondition">Pass in a function that will release the asset when the condition is true. </param>
        /// <typeparam name="T">Type of asset to load.</typeparam>
        /// <returns>The loaded asset.</returns>
        public async UniTask<T> LoadGenericAsset<T>(
            string key,
            Func<bool> releaseCondition,
            Action<T> onSuccess = null,
            Action onFail = null,
            CancellationToken cancellationToken = default
        ) where T : UnityEngine.Object
        {
            var ophandle = Addressables.LoadAssetAsync<T>(key);

            try
            {
                await ophandle.Task;

                _ = ReleaseWhen(releaseCondition, ophandle, cancellationToken);
            }
            catch (Exception e)
            {
                MyLogger.LogError($"Failed to load addressable {key} of type {typeof(T)} with exeception {e}");

                onFail?.Invoke();
            }

            onSuccess?.Invoke(ophandle.Result);

            return ophandle.Result;
        }

        async UniTask ReleaseWhen(
            Func<bool> condition,
            AsyncOperationHandle operationHandle,
            CancellationToken cancellationToken
        )
        {
            await UniTask.WaitUntil(condition, cancellationToken: cancellationToken).SuppressCancellationThrow();

            if (operationHandle.IsValid())
            {
                Addressables.Release(operationHandle);
            }
        }
    }
}