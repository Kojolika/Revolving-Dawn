using Cysharp.Threading.Tasks;
using Models.Characters;
using Systems.Managers;
using UnityEngine;

namespace Views
{
    public class EnemyMoveView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        private Enemy enemy;
        private AddressablesManager addressablesManager;

        [Zenject.Inject]
        private void Construct(Enemy enemy, AddressablesManager addressablesManager)
        {
            this.enemy = enemy;
            this.addressablesManager = addressablesManager;

            enemy.CurrentMoveUpdated += UpdateSprite;
        }

        private void UpdateSprite(EnemyMove enemyMove)
        {
            _ = addressablesManager.LoadGenericAsset(enemyMove.MoveIntentSprite,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset => spriteRenderer.sprite = asset
            );
        }

        private void OnDestroy()
        {
            enemy.CurrentMoveUpdated -= UpdateSprite;
        }
    }
}