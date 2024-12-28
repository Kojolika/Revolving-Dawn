using System;

namespace Fight.Engine.Bytecode
{
    public struct StatType : ICombatByte
    {
        public readonly Type Type;

        public StatType(Type type)
        {
            Type = type;
        }
    }
}