using System.Collections.Generic;
using System.Linq;
using Common.Util;
using Fight.Engine;
using Models.Cards;
using Models.Fight;
using Newtonsoft.Json;
using Tooling.StaticData.Data;

namespace Models.Characters
{
    [System.Serializable]
    public class PlayerCharacter : ICardDeckParticipant
    {
        public PlayerClass Class { get; private set; }

        private readonly CardLogic.Factory cardFactory;

        [JsonConstructor]
        public PlayerCharacter()
        {
        }

        /// <summary>
        /// Used to create characters for a new run.
        /// </summary>
        public PlayerCharacter(
            PlayerClass       playerClass,
            CharacterSettings characterSettings,
            CardLogic.Factory cardFactory)
        {
            Class            = playerClass;
            Name             = playerClass.Name;
            Team             = TeamType.Player;
            this.cardFactory = cardFactory;

            Deck = new();
            foreach (var card in playerClass.StartingDeck)
            {
                Deck.Add(cardFactory.Create(card));
            }

            foreach (var initialStat in characterSettings.InitialStatValues.OrEmptyIfNull())
            {
                SetStat(initialStat.Stat, initialStat.Value);
            }
        }

        #region ICombatParticipant

        public string   Name { get; }
        public TeamType Team { get; }

        private Dictionary<Stat, float> stats = new();
        private Dictionary<Buff, int>   buffs = new();

        public float? GetStat(Stat stat)
        {
            return stats.TryGetValue(stat, out var value) ? value : null;
        }

        public void SetStat(Stat stat, float value)
        {
            stats[stat] = value;
        }

        public int GetBuff(Buff buff)
        {
            return buffs.GetValueOrDefault(buff, 0);
        }

        public void SetBuff(Buff buff, int value)
        {
            buffs[buff] = value;
        }

        public List<(int stackSize, Buff)> GetBuffs()
        {
            var result = new List<(int stackSize, Buff buff)>();
            foreach (var buff in buffs)
            {
                result.Add((buff.Value, buff.Key));
            }

            return result;
        }

        public List<(float amount, Stat)> GetStats()
        {
            var result = new List<(float amount, Stat stat)>();
            foreach (var stat in stats)
            {
                result.Add((stat.Value, stat.Key));
            }

            return result;
        }

        #endregion

        #region ICardDeckParticipant

        public List<CardLogic> Deck    { get; }
        public List<CardLogic> Draw    { get; } = new();
        public List<CardLogic> Hand    { get; } = new();
        public List<CardLogic> Discard { get; } = new();
        public List<CardLogic> Lost    { get; } = new();

        #endregion
    }
}