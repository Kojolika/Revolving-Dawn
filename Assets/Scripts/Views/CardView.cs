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

        public Card Model { get; private set; }

        public Collider       Collider           => cardCollider;
        public SpriteRenderer CardBorderRenderer => cardBorderRenderer;
        public Vector3        DefaultScale       { get; private set; }

        [Inject]
        private void Construct(AddressablesManager addressablesManager, LocalizationManager localizationManager, Card cardModel)
        {
            Model = cardModel;
            nameText.SetText(cardModel.StaticData.Name);

            descriptionText.SetText(localizationManager.Translate(cardModel.StaticData.Description));

            var cancellationToken = this.GetCancellationTokenOnDestroy();


            _ = addressablesManager.LoadGenericAsset(
                assetReference: cardModel.StaticData.PlayerClass.CardBorderArt,
                releaseCondition: () => cancellationToken.IsCancellationRequested,
                onSuccess: asset => cardBorderRenderer.sprite = asset,
                cancellationToken: cancellationToken);


            _ = addressablesManager.LoadGenericAsset(
                cardModel.StaticData.Art,
                () => cancellationToken.IsCancellationRequested,
                asset => cardArtRenderer.sprite = asset, cancellationToken: cancellationToken);
        }

        private void Awake()
        {
            DefaultScale = transform.localScale;
        }

        public class Factory : PlaceholderFactory<Card, CardView>
        {
        }
    }
}