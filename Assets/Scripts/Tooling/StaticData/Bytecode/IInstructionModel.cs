namespace Tooling.StaticData
{
    /// <summary>
    /// We're using the bytecode pattern for our games combat.
    /// </summary>
    public interface IInstructionModel
    {
    }

    /// <summary>
    /// Instruction to default to so we know if there are any errors
    /// </summary>
    public struct Unknown : IInstructionModel
    {
    }
}