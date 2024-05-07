using Cysharp.Threading.Tasks;
using Models.Player;
using Systems.Managers;
using Tooling.Logging;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace UI.Common.DisplayElements
{
    public class ClassDisplayElement : DisplayElement<PlayerClassDefinition>
    {
        [SerializeField] Image classImage;
        [SerializeField] Label className;
        [SerializeField] Label description;
        [SerializeField] MyButton selectbutton;

        public MyButton SelectButton => selectbutton;

        private AssetReference classImageReference;

        public override void Populate(PlayerClassDefinition data)
        {
            className.SetText(data.Name);
            description.SetText(data.Description);

            classImageReference = data.CharacterAvatarKey;

            var asyncOperationHandle = classImageReference.LoadAssetAsync<Sprite>();
            asyncOperationHandle.Completed += (assetHandle) =>
            {
                classImage.sprite = assetHandle.Result;
            };
        }

        private void OnDestroy()
        {
            classImageReference?.ReleaseAsset();
        }
    }
}