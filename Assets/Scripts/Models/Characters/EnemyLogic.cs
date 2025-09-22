using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Controllers.Strategies;
using Fight.Engine;
using Models.Fight;
using Newtonsoft.Json;
using Tooling.StaticData.Data;

namespace Models.Characters
{
    public class EnemyLogic : IMoveParticipant
    {
        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        public TeamType Team { get; private set; }

        [JsonProperty]
        public Enemy Model { get; private set; }

        [JsonProperty]
        public EnemyMove NextMove { get; private set; }

        [JsonProperty]
        public ISelectMoveStrategy SelectMoveStrategy { get; private set; }

        public EnemyLogic(Enemy enemy, ISelectMoveStrategy selectMoveStrategy)
        {
            Model              = enemy;
            SelectMoveStrategy = selectMoveStrategy;
            Name               = enemy.Name;
            Team               = TeamType.Enemy;
        }

        public void SelectMove()
        {
            NextMove = SelectMoveStrategy.SelectMove(Model);
        }

        #region Serialization

        [JsonConstructor]
        private EnemyLogic()
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

        [OnSerializing]
        private void OnSerialized(StreamingContext context)
        {
            serializedStats = stats.Select(kvp => (kvp.Key, kvp.Value)).ToList();
            serializedBuffs = buffs.Select(kvp => (kvp.Key, kvp.Value)).ToList();
        }

        #endregion

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
    }
}