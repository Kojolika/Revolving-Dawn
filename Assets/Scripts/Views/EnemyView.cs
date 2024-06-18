using Models.Characters;
using UnityEngine;
using Zenject;

namespace Views
{
    public class EnemyView : MonoBehaviour
    {
        public readonly EnemyModel enemyModel;
        public EnemyView(EnemyModel enemyModel)
        {
            this.enemyModel = enemyModel;
        }

        public class Factory : PlaceholderFactory<EnemyModel, EnemyView>
        {

        }
    }
}