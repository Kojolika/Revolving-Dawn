namespace Tooling.StaticData.Bytecode
{
    /// <summary>
    /// If the Boolean evaluates to true, execute the next combat byte instruction.
    /// </summary>
    [System.Serializable]
    public struct If : IInstructionModel
    {
        public ExpressionInstruction Condition;
        public Statement IfTrueStatement;
        public Statement ElseStatement;
    }
}