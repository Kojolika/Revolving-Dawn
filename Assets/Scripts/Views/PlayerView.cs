using Cysharp.Threading.Tasks;
using Models.Characters;
using Systems.Managers;
using Tooling.Logging;
using UnityEngine;
using Zenject;

namespace Views
{
    public class PlayerView : MonoBehaviour, ICharacterView, IChangeMaterial
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Transform healthViewLocation;

        public PlayerCharacter PlayerCharacter { get; private set; }

        #region ICharacterView
        public Character Character => PlayerCharacter;
        public Collider Collider { get; private set; }
        public HealthView HealthView { get; private set; }
        public Transform HealthViewLocation => healthViewLocation;
        public SpriteRenderer CharacterRenderer => spriteRenderer;
        #endregion

        [Inject]
        private void Construct(PlayerCharacter playerCharacter, AddressablesManager addressablesManager, HealthView.Factory healthViewFactory)
        {
            PlayerCharacter = playerCharacter;

            _ = addressablesManager.LoadGenericAsset(playerCharacter.Class.CharacterAvatarReference,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset =>
                {
                    spriteRenderer.sprite = asset;
                    Collider = spriteRenderer.gameObject.AddComponent<BoxCollider>();
                    HealthView = healthViewFactory.Create(this);
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