using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Models.Map
{
    [CreateAssetMenu(fileName = "New " + nameof(LevelDefinition), menuName = "RevolvingDawn/Levels/" + nameof(LevelDefinition))]
    public class LevelDefinition : ScriptableObject
    {
        [JsonProperty("level")]
        public int Level;

        [JsonProperty("enemies")]
        public List<EnemyQuantity> PossibleEnemies;
    }
}