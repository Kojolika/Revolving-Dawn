using Cysharp.Threading.Tasks;
using Models.Characters;
using Systems.Managers;
using UnityEngine;
using Zenject;

namespace Views
{
    public class PlayerView : MonoBehaviour, IChangeMaterial
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        public PlayerCharacter PlayerCharacter { get; private set; }

        public void SetMaterial(Material material)
        {
            spriteRenderer.material = material;
        }

        [Inject]
        private void Construct(PlayerCharacter playerCharacter, AddressablesManager addressablesManager)
        {
            PlayerCharacter = playerCharacter;

            _ = addressablesManager.LoadGenericAsset(playerCharacter.Class.CharacterAvatarReference,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset => spriteRenderer.sprite = asset
            );
        }

        public class Factory : PlaceholderFactory<PlayerCharacter, PlayerView> { }
    }
}