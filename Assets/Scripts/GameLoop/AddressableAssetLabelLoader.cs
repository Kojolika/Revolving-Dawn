using System;
using System.Collections.Generic;
using System.Threading;
using Systems.Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace GameLoop
{
    [CreateAssetMenu(fileName = "New " + nameof(AddressableAssetLabelLoader), menuName = "Addressables/" + nameof(AddressableAssetLabelLoader))]
    public class AddressableAssetLabelLoader : ScriptableObject, IInitializable, IDisposable
    {
        [SerializeField] List<AssetLabelReference> assetLabelReferences;

        private AddressablesManager addressablesManager;
        private CancellationTokenSource cts;

        [Inject]
        private void Construct(AddressablesManager addressablesManager)
        {
            this.addressablesManager = addressablesManager;
        }

        public void Initialize()
        {
            cts = new();
            _ = addressablesManager.LoadAssetsFromLabels(assetLabelReferences, 
                () => cts.Token.IsCancellationRequested,
                cancellationToken: cts.Token);
        }

        public void Dispose()
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}