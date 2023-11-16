using Newtonsoft.Json;

namespace Data
{
    [System.Serializable]
    public class LevelDefinition
    {
        [JsonProperty("level_id")]
        public readonly int LevelID;
    }
}