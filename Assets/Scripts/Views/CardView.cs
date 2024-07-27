using Cysharp.Threading.Tasks;
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
        [SerializeField] TextMeshPro nameText;
        [SerializeField] TextMeshPro descriptionText;
        [SerializeField] Collider cardCollider;

        public Models.CardModel Model { get; private set; }

        public Collider Collider => cardCollider;
        public SpriteRenderer CardBorderRenderer => cardBorderRenderer;
        public Vector3 DefaultScale { get; private set; }

        [Inject]
        private void Construct(AddressablesManager addressablesManager, Models.CardModel cardModel)
        {
            this.Model = cardModel;
            nameText.SetText(cardModel.Name);

            var description = "";
            foreach (var cardEffect in cardModel.PlayEffects)
            {
                description += cardEffect.Description;
            }
            descriptionText.SetText(description);

            var cancellationToken = this.GetCancellationTokenOnDestroy();
            _ = addressablesManager.LoadGenericAsset(cardModel.PlayerClass,
                () => cancellationToken.IsCancellationRequested,
                asset =>
                {
                    _ = addressablesManager.LoadGenericAsset(asset.CardBorderReference,
                        () => cancellationToken.IsCancellationRequested,
                        asset => cardBorderRenderer.sprite = asset
                    );
                }
            );

            _ = addressablesManager.LoadGenericAsset(cardModel.ArtReference,
                () => cancellationToken.IsCancellationRequested,
                asset => cardArtRenderer.sprite = asset
            );
        }

        private void Awake()
        {
            DefaultScale = transform.localScale;
        }

        public class Factory : PlaceholderFactory<Models.CardModel, CardView>
        {

        }
    }
}