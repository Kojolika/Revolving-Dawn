using System.Collections.Generic;
using Fight.Engine;
using Models.Fight;
using Newtonsoft.Json;
using Settings;
using Tooling.StaticData;
using Card = Models.Cards.Card;

namespace Models.Characters
{
    [System.Serializable]
    public class PlayerCharacter : IInventory, ICardDeckParticipant
    {
        public PlayerClass Class     { get; private set; }
        public List<IItem> Inventory { get; private set; }
        public ulong       Gold      { get; private set; }

        // TODO: Convert to stats?
        public int HandSize          { get; private set; }
        public int DrawAmount        { get; private set; }
        public int UsableManaPerTurn { get; private set; }

        [JsonConstructor]
        public PlayerCharacter()
        {
        }

        public PlayerCharacter(PlayerClass playerClass, CharacterSettings characterSettings)
        {
            Class             = playerClass;
            Name              = playerClass.Name;
            HandSize          = characterSettings.HandSize;
            DrawAmount        = characterSettings.DrawAmount;
            UsableManaPerTurn = characterSettings.UsableManaPerTurn;
            
            // TODO: Set initial buffs
            //Buffs             = new();
        }

        #region ICombatParticipant

        public string   Name { get; }
        public TeamType Team { get; }

        public bool HasStat(Stat stat)
        {
            throw new System.NotImplementedException();
        }

        public float GetStat(Stat stat)
        {
            throw new System.NotImplementedException();
        }

        public void SetStat(Stat stat, float value)
        {
            throw new System.NotImplementedException();
        }

        public int GetBuff(Buff buff)
        {
            throw new System.NotImplementedException();
        }

        public void SetBuff(Buff buff, int value)
        {
            throw new System.NotImplementedException();
        }

        public List<(int stackSize, Buff)> GetBuffs()
        {
            throw new System.NotImplementedException();
        }

        public List<(float amount, Stat)> GetStats()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region ICardDeckParticipant

        public List<Card> Deck    { get; }
        public List<Card> Draw    { get; }
        public List<Card> Hand    { get; }
        public List<Card> Discard { get; }
        public List<Card> Lost    { get; }

        #endregion
    }
}