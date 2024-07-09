using Newtonsoft.Json;

namespace Models.Characters.Player
{
    [System.Serializable]
    public class PlayerDefinition 
    {
        public PlayerDefinition(int id)
        {
            ID = id;
        }

        [JsonProperty("player_id")] 
        public int ID;
        
        [JsonProperty("current_run")] 
        public RunDefinition CurrentRun;
    }
}