using Newtonsoft.Json;

namespace Data.Definitions.Map
{
    [System.Serializable]
    public class LevelDefinition
    {
        [JsonProperty("level_id")]
        public readonly int LevelID;
    }
}