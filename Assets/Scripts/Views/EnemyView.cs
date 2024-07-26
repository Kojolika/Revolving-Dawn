using Cysharp.Threading.Tasks;
using Models.Characters;
using Systems.Managers;
using UnityEngine;
using Zenject;

namespace Views
{
    public class EnemyView : MonoBehaviour, ICharacterView, IChangeMaterial
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        private HealthView healthView;

        public Enemy Enemy { get; private set; }

        #region ICharacterView
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Character CharacterModel => Enemy;
        public Collider Collider { get; private set; }
        public Renderer Renderer => spriteRenderer;
        #endregion

        [Inject]
        private void Construct(Enemy enemy, AddressablesManager addressablesManager, HealthView.Factory healthViewFactory)
        {
            Enemy = enemy;
            _ = addressablesManager.LoadGenericAsset(enemy.Model.AvatarReference,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset =>
                {
                    spriteRenderer.sprite = asset;
                    Collider = spriteRenderer.gameObject.AddComponent<BoxCollider>();
                    healthView = healthViewFactory.Create(this);
                }
            );
        }

        #region IChangeMaterial
        public void SetMaterial(Material material)
        {
            spriteRenderer.material = material;
        }
        #endregion

        public class Factory : PlaceholderFactory<Enemy, EnemyView> { }
    }
}