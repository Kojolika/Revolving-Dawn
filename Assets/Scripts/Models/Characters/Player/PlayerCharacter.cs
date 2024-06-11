using System.Collections.Generic;
using Models.Player;
using Newtonsoft.Json;

namespace Models.Characters
{
    [System.Serializable]
    public class PlayerCharacter : Character, IInventory
    {
        [JsonProperty("class")]
        public PlayerClassModel Class { get; private set; }

        [JsonProperty("decks")]
        public Player.Decks Decks { get; private set; }

        [JsonProperty("inventory")]
        public List<IItem> Inventory { get; private set; }

        [JsonConstructor]
        public PlayerCharacter()
        {

        }

        public PlayerCharacter(PlayerClassSODefinition playerClassDefinition)
        {
            Class = playerClassDefinition.Representation;
            Name = playerClassDefinition.name;
            Decks = new Player.Decks(Class.StartingDeck);
            Health = new(playerClassDefinition.HealthDefinition.MaxHealth, playerClassDefinition.HealthDefinition.MaxHealth);
        }
    }
}