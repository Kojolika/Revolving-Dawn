using Fight;
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

        private Models.CardModel cardModel;
        private PlayerInputState playerInputState;

        [Inject]
        private void Construct(AddressablesManager addressablesManager, Models.CardModel cardModel, PlayerInputState playerInputState)
        {
            this.cardModel = cardModel;
            this.playerInputState = playerInputState;
            nameText.SetText(cardModel.Name);

            var description = "";
            foreach (var cardEffect in cardModel.PlayEffects)
            {
                description += cardEffect.Description;
            }
            descriptionText.SetText(description);

            _ = addressablesManager.LoadGenericAsset(cardModel.PlayerClass.CardBorderReference,
                () => gameObject == null,
                asset => cardBorderRenderer.sprite = asset
            );

            _ = addressablesManager.LoadGenericAsset(cardModel.ArtReference,
                () => gameObject == null,
                asset => cardArtRenderer.sprite = asset
            );
        }

        private void OnMouseOver()
        {
            playerInputState.CardHovered?.Invoke(this);
        }

        public class Factory : PlaceholderFactory<Models.CardModel, CardView>
        {

        }
    }
}