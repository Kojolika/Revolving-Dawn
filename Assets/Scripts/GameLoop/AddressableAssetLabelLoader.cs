using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace GameLoop
{
    [CreateAssetMenu(fileName = "New " + nameof(AddressableAssetLabelLoader), menuName = "Addressables/" + nameof(AddressableAssetLabelLoader))]
    public class AddressableAssetLabelLoader : ScriptableObject, IInitializable, IDisposable
    {
        [SerializeField] List<AssetLabelReference> assetLabelReferences;

        private AsyncOperationHandle asyncOperationHandle;

        public async UniTask LoadAssetsByLabel()
        {
            asyncOperationHandle = Addressables.LoadResourceLocationsAsync(assetLabelReferences.Select(labelRef => labelRef.labelString), Addressables.MergeMode.Intersection);
            await asyncOperationHandle.Task;
        }

        public void UnloadAssets()
        {
            Addressables.Release(asyncOperationHandle);
        }

        public void Initialize() => _ = LoadAssetsByLabel();
        public void Dispose() => UnloadAssets();
    }
}