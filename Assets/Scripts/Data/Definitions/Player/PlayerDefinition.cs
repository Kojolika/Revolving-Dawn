using Characters.Player2.Run;
using Newtonsoft.Json;

namespace Characters.Model
{
    [System.Serializable]
    public class PlayerDefinition : CharacterMVC.IModel
    {
        public PlayerDefinition(int id)
        {
            ID = id;
        }

        [JsonProperty("player_id")] 
        public int ID { get; }
        
        [JsonProperty("current_run")] 
        public RunDefinition CurrentRun { get; private set; }
    }
}