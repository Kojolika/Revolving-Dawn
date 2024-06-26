using Systems.Managers;
using UnityEngine;
using Zenject;

namespace Views
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        private Models.Player.PlayerClassModel playerClassModel;

        [Inject]
        private void Construct(Models.Player.PlayerClassModel playerClassModel, AddressablesManager addressablesManager)
        {
            this.playerClassModel = playerClassModel;

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