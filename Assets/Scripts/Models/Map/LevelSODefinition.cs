using System.Collections.Generic;
using UnityEngine;

namespace Models.Map
{
    [CreateAssetMenu(fileName = "New " + nameof(LevelSODefinition), menuName = "RevolvingDawn/Map/" + nameof(LevelSODefinition))]
    public class LevelSODefinition : ScriptableObject
    {
        public int Level;
        public List<List<EnemyQuantity>> PossibleEnemies;
    }
}