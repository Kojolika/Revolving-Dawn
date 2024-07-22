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
        [SerializeField] Transform healthViewLocation;

        private PlayerCharacter playerCharacter;
        private BuffsView buffsView;

        #region ICharacterView
        public Character CharacterModel => playerCharacter;
        public Collider Collider { get; private set; }
        public HealthView HealthView { get; private set; }
        public Transform HealthViewLocation => healthViewLocation;
        #endregion

        [Inject]
        private void Construct(PlayerCharacter playerCharacter, 
            AddressablesManager addressablesManager, 
            HealthView.Factory healthViewFactory,
             BuffsView.Factory buffsViewFactory )
        {
            this.playerCharacter = playerCharacter;

            _ = addressablesManager.LoadGenericAsset(playerCharacter.Class.CharacterAvatarReference,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset =>
                {
                    spriteRenderer.sprite = asset;
                    Collider = spriteRenderer.gameObject.AddComponent<BoxCollider>();
                    HealthView = healthViewFactory.Create(this);
                    buffsView = buffsViewFactory.Create(this); 
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