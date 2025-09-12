using System.Collections.Generic;
using Controllers.Strategies;
using Fight.Engine;
using Models.Fight;
using Newtonsoft.Json;
using Tooling.StaticData.Data;

namespace Models.Characters
{
    [System.Serializable]
    public class EnemyLogic : IMoveParticipant
    {
        public string              Name               { get; }
        public TeamType            Team               { get; }
        public Enemy               Model              { get; private set; }
        public EnemyMove           NextMove           { get; private set; }
        public ISelectMoveStrategy SelectMoveStrategy { get; private set; }

        [JsonConstructor]
        private EnemyLogic()
        {
        }

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


        private readonly Dictionary<Stat, float> stats = new();
        private readonly Dictionary<Buff, int>   buffs = new();

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