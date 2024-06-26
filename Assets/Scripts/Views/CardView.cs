using System;
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

        private Models.Card cardModel;

        [Inject]
        private void Construct(AddressablesManager addressablesManager, Models.Card cardModel)
        {
            this.cardModel = cardModel;
            nameText.SetText(cardModel.Name);

            var description = "";
            foreach (var cardEffect in cardModel.PlayEffects)
            {
                description += cardEffect.Description;
            }
            descriptionText.SetText(description);

            _ = addressablesManager.LoadGenericAsset(cardModel.PlayerClass,
                () => gameObject == null,
                asset =>
                {
                    _ = addressablesManager.LoadGenericAsset(asset.CharacterAvatarReference,
                        () => gameObject == null,
                        asset => cardBorderRenderer.sprite = asset
                    );
                }
            );
            _ = addressablesManager.LoadGenericAsset(cardModel.Artwork,
                () => gameObject == null,
                asset => cardArtRenderer.sprite = asset
            );
        }

        public class Factory : PlaceholderFactory<Models.Card, CardView>
        {

        }
    }
}