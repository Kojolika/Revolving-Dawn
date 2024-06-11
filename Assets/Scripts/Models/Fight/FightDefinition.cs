using System.Collections.Generic;
using Models.Characters;
using Newtonsoft.Json;

namespace Models.Fight
{
    [System.Serializable]
    public class FightDefinition
    {
        [JsonProperty("enemies")]
        public List<Enemy> Enemies;
    }
}