namespace Tooling.StaticData.Bytecode
{
    [System.Serializable]
    public struct GetBuff : IInstruction
    {
        public Buff Buff;
        public int Index { get; set; }
    }
}