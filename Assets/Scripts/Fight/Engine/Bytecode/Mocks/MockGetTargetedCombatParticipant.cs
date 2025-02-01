/*using System;
using System.Collections.Generic;
using Tooling.StaticData;
using Tooling.StaticData.Attributes;

namespace Fight.Engine.Bytecode
{
    public struct MockGetTargetedCombatParticipant : IMock, IReduceTo<ICombatParticipant>
    {
        private ICombatParticipant combatParticipant;

        public ICombatParticipant Reduce()
        {
            var rng = new Random();
            var mockedParticipant = new MockCombatParticipant
            {
                Name = CreateRandomName(rng),
                Stats = CreateRandomStats(rng),
                Buffs = CreateRandomBuffs(rng)
            };

            combatParticipant = mockedParticipant;
            return mockedParticipant;
        }


        [GeneralFieldIgnore]
        private class MockCombatParticipant : ICombatParticipant
        {
            public string Name { get; set; }
            public Dictionary<Stat, float> Stats { get; set; }
            public Dictionary<Buff, int> Buffs { get; set; }

            public void Execute(IWorkingMemory workingMemory, IFightContext context, ILogger logger)
            {
                workingMemory.Push(this);
            }
        }

        private static string CreateRandomName(Random rng)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[rng.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        private static Dictionary<Stat, float> CreateRandomStats(Random rng)
        {
            var stats = new Dictionary<Stat, float>();
            var allStats = StaticDatabase.Instance.GetInstancesForType<Stat>();

            foreach (var stat in allStats)
            {
                stats[stat] = rng.Next(0, 1000);
            }

            return stats;
        }

        private static Dictionary<Buff, int> CreateRandomBuffs(Random rng)
        {
            var buffs = new Dictionary<Buff, int>();
            var allBuffs = StaticDatabase.Instance.GetInstancesForType<Buff>();

            foreach (var buff in allBuffs)
            {
                buffs[buff] = rng.Next(0, 1000);
            }

            return buffs;
        }
    }
}*/