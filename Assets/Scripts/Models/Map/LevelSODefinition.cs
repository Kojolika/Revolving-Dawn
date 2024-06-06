using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Models.Map
{
    [CreateAssetMenu(fileName = "New " + nameof(LevelSODefinition), menuName = "RevolvingDawn/Levels/" + nameof(LevelSODefinition))]
    public class LevelSODefinition : ScriptableObject
    {
        [JsonProperty("level")]
        public int Level;

        [JsonProperty("enemies")]
        public List<List<EnemyQuantity>> PossibleEnemies;
    }
}