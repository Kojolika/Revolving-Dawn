using Cysharp.Threading.Tasks;
using Models.Characters;
using Systems.Managers;
using UnityEngine;
using Zenject;

namespace Views
{
    public class PlayerView : MonoBehaviour, ICharacterView, IChangeMaterial
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        private PlayerCharacter playerCharacter;
        private BuffsView buffsView;
        private HealthView healthView;

        #region ICharacterView
        public Character CharacterModel => playerCharacter;
        public Collider Collider { get; private set; }
        public Renderer Renderer => spriteRenderer;
        #endregion

        [Inject]
        private void Construct(PlayerCharacter playerCharacter,
            AddressablesManager addressablesManager,
            HealthView healthView,
            BuffsView buffsView)
        {
            this.playerCharacter = playerCharacter;
            this.healthView = healthView;
            this.buffsView = buffsView;

            _ = addressablesManager.LoadGenericAsset(playerCharacter.Class.CharacterAvatarReference,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset =>
                {
                    spriteRenderer.sprite = asset;
                    Collider = spriteRenderer.gameObject.AddComponent<BoxCollider>();
                }
            );
        }

        #region IChangeMaterial
        public void SetMaterial(Material material)
        {
            spriteRenderer.material = material;
        }
        #endregion

        public class Factory : PlaceholderFactory<PlayerCharacter, PlayerView> { }
    }
}