using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Common.Util;
using Fight.Engine;
using Models.Cards;
using Models.Fight;
using Newtonsoft.Json;
using Tooling.StaticData.Data;

namespace Models.Characters
{
    public class PlayerCharacter : ICardDeckParticipant
    {
        [JsonProperty]
        public PlayerClass Class { get; private set; }

        private readonly CardLogic.Factory cardFactory;

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

        #region Serialization

        [JsonConstructor]
        private PlayerCharacter()
        {
        }

        [JsonProperty]
        private List<(Stat stat, float amount)> serializedStats;

        [JsonProperty]
        private List<(Buff buff, int amount)> serializedBuffs;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            stats = serializedStats.ToDictionary(tuple => tuple.stat, kvp => kvp.amount);
            buffs = serializedBuffs.ToDictionary(tuple => tuple.buff, kvp => kvp.amount);
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            serializedStats = stats.Select(kvp => (kvp.Key, kvp.Value)).ToList();
            serializedBuffs = buffs.Select(kvp => (kvp.Key, kvp.Value)).ToList();
        }

        #endregion

        #region ICombatParticipant

        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        public TeamType Team { get; private set; }

        [JsonIgnore]
        private Dictionary<Stat, float> stats = new();

        [JsonIgnore]
        private Dictionary<Buff, int> buffs = new();

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