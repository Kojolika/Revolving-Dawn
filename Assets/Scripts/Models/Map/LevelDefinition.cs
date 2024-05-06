﻿using System.Collections.Generic;
using Models.Characters;
using Newtonsoft.Json;
using UnityEngine;

namespace Models.Map
{
    [System.Serializable, CreateAssetMenu(fileName = "New Level Definition", menuName = "RevolvingDawn/Levels/Level")]
    public class LevelDefinition : ScriptableObject
    {
        [JsonProperty("level")]
        public int Level;

        [JsonProperty("enemies")]
        public List<Enemy> PossibleEnemies;
    }
}