namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// If the Boolean evaluates to true, execute the next combat byte instruction.
    /// </summary>
    public struct If : IPop<Boolean, ICombatByte>, IReduceTo<ICombatByte>
    {
        private ICombatByte nextInstruction;

        public void OnBytesPopped(Boolean condition, ICombatByte nextInstruction)
        {
            this.nextInstruction = condition.Value
                ? nextInstruction
                : null;
        }

        public ICombatByte Reduce() => nextInstruction;


        public string Log()
        {
            return nextInstruction?.Log();
        }
    }
}