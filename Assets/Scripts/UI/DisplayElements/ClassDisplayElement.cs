using Data.Definitions.Player;
using Systems.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common.DisplayElements
{
    public class ClassDisplayElement : DisplayElement<ClassDefinition>
    {
        [SerializeField] Image classImage;
        [SerializeField] Label className;
        [SerializeField] Label description;
        [SerializeField] MyButton selectbutton;

        public MyButton SelectButton => selectbutton;
        public static readonly string PlaceholderCharacterKey = "placeholder-character";

        private AddressablesManager addressablesManager;

        [Zenject.Inject]
        void Construct(AddressablesManager addressablesManager)
        {
            this.addressablesManager = addressablesManager;
        }

        public override async void Populate(ClassDefinition data)
        {
            className.SetText(data.Name);
            description.SetText(data.Description);

            var classSprite = data.characterSprite;

            classImage.sprite = classSprite != null
                ? classSprite
                : await addressablesManager.LoadGenericAsset<Sprite>(
                    PlaceholderCharacterKey,
                    () => gameObject == null
                );
        }
    }
}