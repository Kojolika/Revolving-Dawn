using System.Collections.Generic;
using Tooling.StaticData;

namespace Fight.Engine.Bytecode
{
    public interface ICombatParticipant : IStoreable
    {
        string Name { get; }
        Dictionary<Stat, float> Stats { get; }
        Dictionary<Buff, int> Buffs { get; }
    }
}