using System.Collections.Generic;
using Tooling.Logging;
using Tooling.StaticData;

namespace Fight.Engine.Bytecode
{
    public interface ICombatParticipant : ICombatByte
    {
        string Name { get; }
        Dictionary<Stat, float> Stats { get; }
        Dictionary<Buff, int> Buffs { get; }

        string ICombatByte.Log()
        {
            return Name;
        }
    }

    public class MockCombatParticipant : ICombatParticipant
    {
        public MockCombatParticipant()
        {
            MyLogger.Log($"Creating {Name}");
            var rng = new System.Random();

            Stats = new Dictionary<Stat, float>();
            foreach (var stat in StaticDatabase.Instance.GetInstancesForType<Stat>())
            {
                var statValue = (float)rng.Next(0, 100);
                MyLogger.Log($"Adding {stat.Name} with value {statValue}");
                Stats[stat] = statValue;
            }

            Buffs = new Dictionary<Buff, int>();
            foreach (var buff in StaticDatabase.Instance.GetInstancesForType<Buff>())
            {
                var buffValue = rng.Next(0, 100);
                MyLogger.Log($"Adding {buff.Name} with value {buffValue}");
                Buffs[buff] = buffValue;
            }
        }

        public string Name => "Mock_CombatParticipant";
        public Dictionary<Stat, float> Stats { get; }
        public Dictionary<Buff, int> Buffs { get; }
    }
}