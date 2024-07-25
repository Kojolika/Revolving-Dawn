using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Systems.Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;

namespace GameLoop
{
    [CreateAssetMenu(fileName = "New " + nameof(AddressableAssetLabelLoader), menuName = "Addressables/" + nameof(AddressableAssetLabelLoader))]
    public class AddressableAssetLabelLoader : ScriptableObject, IInitializable, IDisposable
    {
        [SerializeField] List<AssetLabelReference> assetLabelReferences;

        private AsyncOperationHandle<IList<IResourceLocation>> resourceLocationsHandle;
        private AddressablesManager addressablesManager;
        private CancellationTokenSource cts;

        [Inject]
        private void Construct(AddressablesManager addressablesManager)
        {
            this.addressablesManager = addressablesManager;
        }

        public async UniTask LoadAssetsByLabel()
        {
            resourceLocationsHandle = Addressables.LoadResourceLocationsAsync(assetLabelReferences.Select(labelRef => labelRef.labelString), Addressables.MergeMode.Union);
            await resourceLocationsHandle.Task;
            cts = new();
            
            int resourceLocationCount = resourceLocationsHandle.Result.Count;
            var loadTasks = new UniTask[resourceLocationCount];
            for (int i = 0; i < resourceLocationCount; i++)
            {
                var resourceLocation = resourceLocationsHandle.Result[i];
                loadTasks[i] = addressablesManager.LoadGenericAsset<UnityEngine.Object>(resourceLocation, () => cts.Token.IsCancellationRequested);
            }
            await UniTask.WhenAll(loadTasks).SuppressCancellationThrow();
        }

        public void UnloadAssets()
        {
            Addressables.Release(resourceLocationsHandle);
            cts.Cancel();
            cts.Dispose();
        }

        public void Initialize() => _ = LoadAssetsByLabel();
        public void Dispose() => UnloadAssets();
    }
}