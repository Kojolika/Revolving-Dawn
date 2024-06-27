using System.Collections.Generic;
using Models.Player;
using Newtonsoft.Json;
using Settings;

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

        [JsonProperty("gold")]
        public ulong Gold { get; private set; }

        [JsonProperty("hand_size")]
        public int HandSize { get; private set; }

        [JsonProperty("draw_amount")]
        public int DrawAmount { get; private set; }

        [JsonProperty("mana_per_turn")]
        public int UsableManaPerTurn {get; private set; }

        [JsonConstructor]
        public PlayerCharacter()
        {

        }

        public PlayerCharacter(PlayerClassSODefinition playerClassDefinition, CharacterSettings characterSettings)
        {
            Class = playerClassDefinition.Representation;
            Name = playerClassDefinition.name;
            Decks = new Player.Decks(Class.StartingDeck);
            Health = new(playerClassDefinition.HealthDefinition.MaxHealth, playerClassDefinition.HealthDefinition.MaxHealth);
            HandSize = characterSettings.HandSize;
            DrawAmount = characterSettings.DrawAmount;
            UsableManaPerTurn = characterSettings.UsableManaPerTurn;
        }
    }
}