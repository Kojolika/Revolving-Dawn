using Tooling.StaticData;

namespace Fight.Engine.Bytecode
{
    public struct GetBuff : IPop<ICombatParticipant, Buff>, IReduceTo<Literal>
    {
        private Literal buffValue;
        private ICombatParticipant target;
        private Buff buff;

        public void OnBytesPopped(ICombatParticipant target, Buff buff)
        {
            if (target.Buffs.TryGetValue(buff, out var statCount))
            {
                buffValue = new Literal(statCount);
            }
        }

        public Literal Reduce() => buffValue;

        public string Log()
        {
            return $"{buffValue}: stacks of {buff} on {target?.Name}";
        }
    }
}