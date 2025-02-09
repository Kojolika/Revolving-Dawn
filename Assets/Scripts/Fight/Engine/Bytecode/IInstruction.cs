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

    /// <summary>
    /// Helper interface to display the type popped into memory in the <see cref="Tooling.StaticData.EditorUI.EditorWindow"/>
    /// </summary>
    public interface IPop<T> : IInstruction
    {
    }

    /// <summary>
    /// Helper interface to display the types popped into memory in the <see cref="Tooling.StaticData.EditorUI.EditorWindow"/>
    /// </summary>
    public interface IPop<T1, T2> : IInstruction
    {
    }

    /// <summary>
    /// Helper interface to display the types pushed into memory in the <see cref="Tooling.StaticData.EditorUI.EditorWindow"/>
    /// </summary>
    public interface IPush<T> : IInstruction
    {
    }
}