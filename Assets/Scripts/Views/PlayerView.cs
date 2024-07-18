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

        public PlayerCharacter PlayerCharacter { get; private set; }

        #region ICharacterView
        public Character Character => PlayerCharacter;
        public Collider Collider { get; private set; }
        public HealthView HealthView { get; private set; }
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
                    HealthView = healthViewFactory.Create(Character.Health, this);
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