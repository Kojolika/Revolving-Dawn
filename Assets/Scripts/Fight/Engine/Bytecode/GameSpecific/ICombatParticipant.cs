using System.Collections.Generic;
using Tooling.StaticData;

namespace Fight.Engine.Bytecode
{
    public interface ICombatParticipant : ICombatByte
    {
        Dictionary<Stat, float> Stats { get; }
        Dictionary<Buff, int> Buffs { get; }
    }
}