namespace Tooling.StaticData.Bytecode
{
    /// <summary>
    /// Gets the stat value for a combat participant.
    /// </summary>
    [System.Serializable]
    public struct GetStat : IInstruction
    {
        public Stat Stat;
        public int Index { get; set; }
    }
}