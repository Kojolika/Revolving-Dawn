using Tooling.StaticData.EditorUI;

namespace Tooling.StaticData.Data
{
    /// <summary>
    /// We're using the bytecode pattern for our games combat.
    /// </summary>
    public abstract class InstructionModel
    {
        /// <summary>
        /// Guarantees a public constructor for types that inherit this.
        /// We create types at runtime in <see cref="StatementStaticDataDrawer"/> with <see cref="System.Activator"/>
        /// </summary>
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