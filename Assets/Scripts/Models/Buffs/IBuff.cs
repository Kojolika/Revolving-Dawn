namespace Models.Buffs
{
    public interface IBuff
    {
        string Name { get; }
        ulong MaxStackSize { get; }
        ulong CurrentStackSize { get; }
    }
}