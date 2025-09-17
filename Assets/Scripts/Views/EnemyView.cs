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

        private EnemyLogic enemyLogic;

        #region ICharacterView

        public SpriteRenderer     SpriteRenderer => spriteRenderer;
        public ICombatParticipant Model          => enemyLogic;
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
            EnemyLogic          enemyLogic,
            HealthView          healthView,
            EnemyMoveView       enemyMoveView,
            BuffsView           buffsView,
            AddressablesManager addressablesManager)
        {
            this.enemyLogic = enemyLogic;

            _ = addressablesManager.LoadGenericAsset(
                enemyLogic.Model.Image,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset =>
                {
                    spriteRenderer.sprite = asset;
                    Collider              = spriteRenderer.gameObject.AddComponent<BoxCollider>();
                }
            );
        }

        public class Factory : PlaceholderFactory<EnemyLogic, EnemyView>
        {
        }
    }
}