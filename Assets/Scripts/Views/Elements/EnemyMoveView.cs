using Cysharp.Threading.Tasks;
using Models.Characters;
using Systems.Managers;
using Tooling.StaticData.EditorUI;
using UnityEngine;

namespace Views
{
    public class EnemyMoveView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        private EnemyLogic               enemyLogic;
        private AddressablesManager addressablesManager;

        [Zenject.Inject]
        private void Construct(EnemyLogic enemyLogic, AddressablesManager addressablesManager)
        {
            this.enemyLogic               = enemyLogic;
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