namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Statements that can have things trigger before or after this byte executes
    /// </summary>
    public interface ITriggerPoint : IInstruction
    {
    }
}