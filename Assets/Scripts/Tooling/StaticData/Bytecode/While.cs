namespace Tooling.StaticData.Bytecode
{
    [System.Serializable]
    public struct While : IInstruction
    {
        public ExpressionInstruction Condition;
        public Statement Statement;
        public int Index { get; set; }
    }
}