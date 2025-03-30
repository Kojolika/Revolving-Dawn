namespace Tooling.StaticData.Bytecode
{
    /// <summary>
    /// Sets the stat value for a combat participant.
    /// </summary>
    [System.Serializable]
    public struct SetStat : IInstruction
    {
        public Stat Stat;
        public int Index { get; set; }
    }
}