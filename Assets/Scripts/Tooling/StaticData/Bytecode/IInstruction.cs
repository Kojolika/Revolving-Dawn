namespace Tooling.StaticData
{
    /// <summary>
    /// We're using the bytecode pattern for our games combat.
    /// </summary>
    public interface IInstruction
    {
        /// <summary>
        /// Think of this like a line number in a program
        /// </summary>
        int Index { get; set; }
    }
}