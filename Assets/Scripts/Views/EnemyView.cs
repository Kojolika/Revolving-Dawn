using Models.Characters;
using Systems.Managers;
using UnityEngine;
using Zenject;

namespace Views
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        private Enemy enemy;

        [Inject]
        private void Construct(Enemy enemy, AddressablesManager addressablesManager)
        {
            this.enemy = enemy;
            _ = addressablesManager.LoadGenericAsset(enemy.Model.AvatarReference,
                () => gameObject == null,
                asset => spriteRenderer.sprite = asset 
            );
        }

        public class Factory : PlaceholderFactory<Enemy, EnemyView>
        {

        }
    }
}