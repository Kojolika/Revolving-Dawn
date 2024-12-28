namespace Fight.Engine.Bytecode
{
    public interface ITriggerBefore<T>
        where T : ICombatByte
    {
    }

    public interface ITriggerAfter<T>
        where T : ICombatByte
    {
    }
}