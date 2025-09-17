using Cysharp.Threading.Tasks;
using Systems.Managers;
using Tooling.StaticData.Data;
using UnityEngine;
using UnityEngine.UI;
using Views.Common;
using Zenject;

namespace UI.Common.DisplayElements
{
    public class PlayerClassView : MonoBehaviour, IView<PlayerClass>
    {
        [SerializeField] Image    classImage;
        [SerializeField] Label    className;
        [SerializeField] Label    description;
        [SerializeField] MyButton selectbutton;

        public MyButton SelectButton => selectbutton;

        private AddressablesManager addressablesManager;
        private LocalizationManager localizationManager;

        [Inject]
        private void Construct(AddressablesManager addressablesManager, LocalizationManager localizationManager)
        {
            this.addressablesManager = addressablesManager;
            this.localizationManager = localizationManager;
        }

        public void Populate(PlayerClass data)
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