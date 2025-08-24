using System.Collections.Generic;
using System.Linq;
using Models.Buffs;
using Models.Player;
using Newtonsoft.Json;
using Settings;
using UnityEngine.AddressableAssets;

namespace Models.Characters
{
    [System.Serializable]
    public class PlayerCharacter : Character, IInventory
    {
        public PlayerClassModel Class             { get; private set; }
        public Player.Decks     Decks             { get; private set; }
        public List<IItem>      Inventory         { get; private set; }
        public ulong            Gold              { get; private set; }
        public int              HandSize          { get; private set; }
        public int              DrawAmount        { get; private set; }
        public int              UsableManaPerTurn { get; private set; }

        [JsonConstructor]
        public PlayerCharacter()
        {
        }

        public PlayerCharacter(PlayerClassSODefinition playerClassDefinition, CharacterSettings characterSettings)
        {
            Class             = playerClassDefinition.Representation;
            Name              = playerClassDefinition.name;
            Decks             = new Player.Decks(playerClassDefinition.StartingDeck.Select(cardSO => cardSO.Representation).ToList());
            Health            = new(playerClassDefinition.HealthDefinition.MaxHealth, playerClassDefinition.HealthDefinition.MaxHealth);
            HandSize          = characterSettings.HandSize;
            DrawAmount        = characterSettings.DrawAmount;
            UsableManaPerTurn = characterSettings.UsableManaPerTurn;
            Buffs             = new();
        }
    }
}