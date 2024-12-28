namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// If the Boolean evaluates to true, execute the next combat byte instruction.
    /// </summary>
    public struct If : IPopByte<Boolean, ICombatByte>,
        IPushByte<ICombatByte>
    {
        private ICombatByte nextInstruction;

        public void Pop(Boolean input, ICombatByte input2)
        {
            nextInstruction = input.Value
                ? input2
                : null;
        }

        public ICombatByte Push() => nextInstruction;
    }
}