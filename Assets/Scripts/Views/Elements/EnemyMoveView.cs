using Cysharp.Threading.Tasks;
using Systems.Managers;
using Tooling.StaticData;
using UnityEngine;
using Enemy = Models.Characters.Enemy;

namespace Views
{
    public class EnemyMoveView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        private Enemy               enemy;
        private AddressablesManager addressablesManager;

        [Zenject.Inject]
        private void Construct(Enemy enemy, AddressablesManager addressablesManager)
        {
            this.enemy               = enemy;
            this.addressablesManager = addressablesManager;

            // TODO: Add battle event animation and subscribe to when that triggers
        }

        private void UpdateSprite(EnemyMove enemyMove)
        {
            _ = addressablesManager.LoadGenericAsset(
                enemyMove.MoveIntentSprite,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset => spriteRenderer.sprite = asset
            );
        }
    }
}