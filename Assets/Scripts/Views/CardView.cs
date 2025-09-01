using Cysharp.Threading.Tasks;
using Models.Cards;
using Settings;
using Systems.Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace Views
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer cardBorderRenderer;
        [SerializeField] SpriteRenderer cardArtRenderer;
        [SerializeField] TextMeshPro    nameText;
        [SerializeField] TextMeshPro    descriptionText;
        [SerializeField] Collider       cardCollider;

        public CardLogic Model { get; private set; }

        public Collider       Collider           => cardCollider;
        public SpriteRenderer CardBorderRenderer => cardBorderRenderer;
        public Vector3        DefaultScale       { get; private set; }

        [Inject]
        private void Construct(AddressablesManager addressablesManager, LocalizationManager localizationManager, CardLogic cardLogicModel)
        {
            Model = cardLogicModel;
            nameText.SetText(cardLogicModel.Model.Name);

            descriptionText.SetText(localizationManager.Translate(cardLogicModel.Model.Description));

            var cancellationToken = this.GetCancellationTokenOnDestroy();


            _ = addressablesManager.LoadGenericAsset(
                assetReference: cardLogicModel.Model.PlayerClass.CardBorderArt,
                releaseCondition: () => cancellationToken.IsCancellationRequested,
                onSuccess: asset => cardBorderRenderer.sprite = asset,
                cancellationToken: cancellationToken);


            _ = addressablesManager.LoadGenericAsset(
                cardLogicModel.Model.Art,
                () => cancellationToken.IsCancellationRequested,
                asset => cardArtRenderer.sprite = asset, cancellationToken: cancellationToken);
        }

        private void Awake()
        {
            DefaultScale = transform.localScale;
        }

        public class Factory : PlaceholderFactory<CardLogic, CardView>
        {
        }
    }
}