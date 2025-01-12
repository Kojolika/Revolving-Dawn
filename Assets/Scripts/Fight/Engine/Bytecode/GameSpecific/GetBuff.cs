using Tooling.StaticData;

namespace Fight.Engine.Bytecode
{
    public struct GetBuff : IPop<ICombatParticipant, Buff>, IPush<Literal>
    {
        private Literal buffValue;

        public void Pop(ICombatParticipant input, Buff input2)
        {
            if (input.Buffs.TryGetValue(input2, out var statCount))
            {
                buffValue = new Literal(statCount);
            }
        }

        public Literal Push() => buffValue;
    }
}