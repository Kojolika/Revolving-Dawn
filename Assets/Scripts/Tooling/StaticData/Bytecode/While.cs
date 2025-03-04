namespace Tooling.StaticData
{
    [System.Serializable]
    public struct While : IInstruction
    {
        public ExpressionInstruction Condition;
        public Statement Statement;
    }
}