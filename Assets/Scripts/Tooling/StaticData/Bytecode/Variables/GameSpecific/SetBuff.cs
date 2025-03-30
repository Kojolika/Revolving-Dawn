namespace Tooling.StaticData.Bytecode
{
    /// <summary>
    /// Sets the stat value for a combat participant.
    /// </summary>
    [System.Serializable]
    public struct SetBuff : IInstruction
    {
        public Buff Buff;
        public int Index { get; set; }
    }
}