using System.Collections.Generic;
using Models.Characters;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

namespace Models.Map
{
    [System.Serializable, CreateAssetMenu(fileName = "New Level Definition", menuName = "RevolvingDawn/Levels/Level")]
    public class LevelDefinition : ScriptableObject
    {
        [JsonProperty("level")]
        public int Level;

        public List<Enemy> PossibleEnemies;
    }
}