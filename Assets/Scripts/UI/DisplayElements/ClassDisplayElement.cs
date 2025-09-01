using Cysharp.Threading.Tasks;
using Systems.Managers;
using Tooling.StaticData.EditorUI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Common.DisplayElements
{
    public class ClassDisplayElement : DisplayElement<PlayerClass>
    {
        [SerializeField] Image    classImage;
        [SerializeField] Label    className;
        [SerializeField] Label    description;
        [SerializeField] MyButton selectbutton;

        public MyButton SelectButton => selectbutton;

        private AddressablesManager addressablesManager;
        private LocalizationManager localizationManager;

        // TODO: Set up ClassDisplayElement in DiContainer so it can have it dependencies injected
        [Inject]
        private void Construct(AddressablesManager addressablesManager, LocalizationManager localizationManager)
        {
            this.addressablesManager = addressablesManager;
            this.localizationManager = localizationManager;
        }

        public override void Populate(PlayerClass data)
        {
            className.SetText(data.Name);
            description.SetText(localizationManager.Translate(data.Description));

            _ = addressablesManager.LoadGenericAsset(
                data.ClassArt,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                onSuccess: sprite => classImage.sprite = sprite
            );
        }
    }
}