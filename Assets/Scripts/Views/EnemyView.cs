using Cysharp.Threading.Tasks;
using Models.Characters;
using Systems.Managers;
using UnityEngine;
using Zenject;

namespace Views
{
    public class EnemyView : MonoBehaviour, IChangeMaterial
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        private Enemy enemy;

        public void SetMaterial(Material material)
        {
            spriteRenderer.material = material;
        }

        [Inject]
        private void Construct(Enemy enemy, AddressablesManager addressablesManager)
        {
            this.enemy = enemy;
            _ = addressablesManager.LoadGenericAsset(enemy.Model.AvatarReference,
                () => this.GetCancellationTokenOnDestroy().IsCancellationRequested,
                asset => spriteRenderer.sprite = asset 
            );
        }

        public class Factory : PlaceholderFactory<Enemy, EnemyView>
        {

        }
    }
}