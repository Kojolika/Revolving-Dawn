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
        private EnemyMoveView enemyMoveView;
        private BuffsView buffsView;

        public Enemy Enemy { get; private set; }

        #region ICharacterView
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Character CharacterModel => Enemy;
        public Collider Collider { get; private set; }
        public Renderer Renderer => spriteRenderer;
        #endregion

        [Inject]
        private void Construct(
            Enemy enemy,
            HealthView healthView,
            EnemyMoveView enemyMoveView,
            BuffsView buffsView,
            AddressablesManager addressablesManager)
        {
            Enemy = enemy;
            this.healthView = healthView;
            this.enemyMoveView = enemyMoveView;
            this.buffsView = buffsView;

            _ = addressablesManager.LoadGenericAsset(enemy.Model.AvatarReference,
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

        public class Factory : PlaceholderFactory<Enemy, EnemyView> { }
    }
}