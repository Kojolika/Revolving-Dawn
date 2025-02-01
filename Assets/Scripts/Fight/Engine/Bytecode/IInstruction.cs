namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// We're using the bytecode pattern for our games combat.
    /// </summary>
    public interface IInstruction
    {
        // TODO: Loc
        /// <summary>
        /// Defines what this instruction does when the <see cref="Interpreter"/> reads it as input.
        /// </summary>
        /// <param name="context">The current execution context</param>
        void Execute(Context context);
    }
}