using System.Collections.Generic;
using Tooling.StaticData;
using Tooling.StaticData.Bytecode;

namespace Fight.Engine
{
    [Object]
    public interface ICombatParticipant
    {
        [Function(Type.String)]
        string Name { get; }

        [Function(Type.Float, inputs: Type.Object)]
        Dictionary<Stat, float> Stats { get; }

        [Function(Type.Int, inputs: Type.Object)]
        Dictionary<Buff, int> Buffs { get; }
    }
}