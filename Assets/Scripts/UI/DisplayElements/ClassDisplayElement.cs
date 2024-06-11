using Models.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI.Common.DisplayElements
{
    public class ClassDisplayElement : DisplayElement<PlayerClassSODefinition>
    {
        [SerializeField] Image classImage;
        [SerializeField] Label className;
        [SerializeField] Label description;
        [SerializeField] MyButton selectbutton;

        public MyButton SelectButton => selectbutton;

        private AssetReference classImageReference;

        public override void Populate(PlayerClassSODefinition data)
        {
            className.SetText(data.name);
            description.SetText(data.Description);

            classImageReference = data.CharacterAvatarReference;

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