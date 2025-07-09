using System.Collections.Generic;
using Tooling.StaticData;
using Tooling.StaticData.Bytecode;

namespace Fight.Engine
{
    [ByteObject]
    public interface ICombatParticipant
    {
        [ByteProperty(Type.String)]
        string Name { get; }

        [ByteFunction(Type.Float, inputs: Type.Object)]
        float GetStat(Stat stat);

        [ByteFunction(Type.Int, inputs: Type.Object)]
        int GetBuff(Buff buff);
    }
}