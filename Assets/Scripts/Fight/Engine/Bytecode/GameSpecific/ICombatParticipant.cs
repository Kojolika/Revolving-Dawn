using System.Collections.Generic;

namespace Fight.Engine.Bytecode
{
    public interface ICombatParticipant : ICombatByte
    {
        Dictionary<System.Type, Stat> Stats { get; }
        Dictionary<System.Type, Buff> Buffs { get; }
    }
}