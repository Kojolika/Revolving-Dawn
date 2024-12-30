namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Combat bytes that can have things trigger before or after this byte executes
    /// </summary>
    public interface ITriggerPoint : ICombatByte
    {
    }
}