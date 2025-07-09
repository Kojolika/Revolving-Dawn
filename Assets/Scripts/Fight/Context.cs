using System.Collections.Generic;
using Fight.Engine;
using Tooling.StaticData.Bytecode;

namespace Fight
{
    [ByteCodeContainer]
    public class Context
    {
        [ByteFunction(Type.Object)]
        public List<ICombatParticipant> GetAllCombatParticipants()
        {
            List<ICombatParticipant> list = new List<ICombatParticipant>();

            return list;
        }
    }
}