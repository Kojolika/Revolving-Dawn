using System.Collections.Generic;
using Fight.Engine;
using Tooling.StaticData.Bytecode;

namespace Fight
{
    public class Context
    {
        [Function(Type.Object)]
        public List<ICombatParticipant> GetAllCombatParticipants()
        {
            List<ICombatParticipant> list = new List<ICombatParticipant>();

            return list;
        }
    }
}