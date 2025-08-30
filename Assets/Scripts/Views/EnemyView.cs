using Cysharp.Threading.Tasks;
using Fight.Engine;
using Models.Characters;
using Systems.Managers;
using UnityEngine;
using Zenject;

namespace Views
{
    public class EnemyView : MonoBehaviour, IParticipantView
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        private HealthView    healthView;
        private EnemyMoveView enemyMoveView;
        private BuffsView     buffsView;

        public Enemy EnemyModel { get; private set; }

        #region ICharacterView

        public SpriteRenderer     SpriteRenderer => spriteRenderer;
        public ICombatParticipant Model          => EnemyModel;
        public Collider           Collider       { get; private set; }
        public Renderer           Renderer       => spriteRenderer;

        public void Highlight()
        {
            throw new System.NotImplementedException();
        }

        public void HighlightFriendly()
        {
            throw new System.NotImplementedException();
        }

        public void HighlightEnemy()
        {
            throw new System.NotImplementedException();
        }

        public void Unhighlight()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        [Inject]
        private void Construct(
            Enemy               enemy,
            HealthView          healthView,
            EnemyMoveView       enemyMoveView,
            BuffsView           buffsView,
            AddressablesManager addressablesManager)
        {
            EnemyModel         = enemy;
            this.healthView    = healthView;
            this.enemyMoveView = enemyMoveView;
            this.buffsView     = buffsView;

            _ = addressablesManager.LoadGenericAsset(
                enemy.Model.AvatarReference,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset =>
                {
                    spriteRenderer.sprite = asset;
                    Collider              = spriteRenderer.gameObject.AddComponent<BoxCollider>();
                }
            );
        }

        #region IChangeMaterial

        public void SetMaterial(Material material)
        {
            spriteRenderer.material = material;
        }

        #endregion

        public class Factory : PlaceholderFactory<Enemy, EnemyView>
        {
        }
    }
}