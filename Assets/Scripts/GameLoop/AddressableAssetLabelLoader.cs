using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Systems.Managers;
using Tooling.Logging;
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
            cts = new();
            await addressablesManager.LoadAssetsFromLabels(assetLabelReferences, () => cts.Token.IsCancellationRequested, cancellationToken: cts.Token);
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