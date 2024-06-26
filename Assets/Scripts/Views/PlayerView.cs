using Systems.Managers;
using UnityEngine;
using Zenject;

namespace Views
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        public Models.Player.PlayerClassModel PlayerClassModel { get; private set; }

        [Inject]
        private void Construct(Models.Player.PlayerClassModel playerClassModel, AddressablesManager addressablesManager)
        {
            this.PlayerClassModel = playerClassModel;

            _ = addressablesManager.LoadGenericAsset(playerClassModel.CharacterAvatarReference,
                () => gameObject == null,
                asset => spriteRenderer.sprite = asset
            );
        }

        public class Factory : PlaceholderFactory<Models.Player.PlayerClassModel, PlayerView>
        {

        }
    }
}