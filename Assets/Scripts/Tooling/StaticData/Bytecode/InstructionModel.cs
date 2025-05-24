namespace Tooling.StaticData
{
    /// <summary>
    /// We're using the bytecode pattern for our games combat.
    /// </summary>
    public abstract class InstructionModel
    {
        public InstructionModel()
        {
        }
    }

    /// <summary>
    /// Instruction to default to so we know if there are any errors
    /// </summary>
    public class Unknown : InstructionModel
    {
    }
}