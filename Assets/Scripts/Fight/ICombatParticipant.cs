using System.Collections.Generic;
using Tooling.StaticData;

namespace Fight.Engine
{
    public interface ICombatParticipant
    {
        string Name { get; }
        Dictionary<Stat, float> Stats { get; }
        Dictionary<Buff, int> Buffs { get; }
    }
}