using System.Collections.Generic;
using Models.Health;
using Models.Player;
using Newtonsoft.Json;

namespace Models.Characters
{
    [System.Serializable]
    public class PlayerCharacter : Character, IInventory
    {
        [JsonProperty("class")]
        public PlayerClass Class { get; private set; }

        [JsonProperty("decks")]
        public Player.Decks Decks { get; private set; }

        [JsonProperty("inventory")]
        public List<IItem> Inventory { get; private set; }

        [JsonIgnore]
        public override string Name => Class.Name;

        [JsonIgnore]
        public override HealthDefinition HealthDefinition => Class.HealthDefinition;

        [JsonConstructor]
        public PlayerCharacter()
        {

        }

        public PlayerCharacter(PlayerClassSODefinition playerClassDefinition)
        {
            Class = playerClassDefinition.Representation;
            Decks = new Player.Decks(Class.StartingDeck);
            Health = new(HealthDefinition.MaxHealth, HealthDefinition);
        }
    }
}