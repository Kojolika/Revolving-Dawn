using System.Collections.Generic;
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


        [JsonConstructor]
        public PlayerCharacter()
        {

        }

        public PlayerCharacter(PlayerClassSODefinition playerClassDefinition)
        {
            Class = playerClassDefinition.Representation;
            Decks = new Player.Decks(Class.StartingDeck);
            Health = new(playerClassDefinition.HealthDefinition.MaxHealth, playerClassDefinition.HealthDefinition.MaxHealth);
        }
    }
}