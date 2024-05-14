using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Data
{
    [Serializable]
    public class StaticDataReference<T> where T : UnityEngine.Object
    {
        [UnityEngine.SerializeField] private List<AssetReferenceT<T>> assetReferences;

        public List<AssetReferenceT<T>> AssetReferences => assetReferences;

        public async UniTask<List<T>> LoadAssetsAsync()
        {
            var tasks = assetReferences
                .Select(assetReference => assetReference.LoadAssetAsync<T>())
                .Select(async asyncOperationHandle => await asyncOperationHandle.Task)
                .ToList();

            return new List<T>(await UniTask.WhenAll(tasks));
        }

        public List<T> LoadAssetsSync()
            => assetReferences
                .Select(assetReference => assetReference.LoadAssetAsync<T>().WaitForCompletion())
                .ToList();
    }
}