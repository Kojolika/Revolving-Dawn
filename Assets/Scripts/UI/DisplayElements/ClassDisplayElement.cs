using Cysharp.Threading.Tasks;
using Models.Player;
using Systems.Managers;
using Tooling.Logging;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

        private AddressablesManager addressablesManager;

        [Zenject.Inject]
        void Construct(AddressablesManager addressablesManager)
        {
            this.addressablesManager = addressablesManager;
        }

        public override async void Populate(PlayerClassDefinition data)
        {
            className.SetText(data.Name);
            description.SetText(data.Description);

            MyLogger.Log($"key: {data.CharacterAvatarKey}");

            await UniTask.WaitUntil(() => addressablesManager != null);

            _ = addressablesManager.LoadGenericAsset<Sprite>(
                data.CharacterAvatarKey,
                () => gameObject == null,
                (asset) => classImage.sprite = Instantiate(asset)
            );
        }
    }
}