namespace Tooling.StaticData
{
    /// <summary>
    /// If the Boolean evaluates to true, execute the next combat byte instruction.
    /// </summary>
    [System.Serializable]
    public struct If : IInstruction
    {
        public ExpressionInstruction Condition;
        public Statement IfTrueStatement;
        public Statement ElseStatement;
    }
}