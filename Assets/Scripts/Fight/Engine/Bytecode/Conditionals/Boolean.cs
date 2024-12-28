namespace Fight.Engine.Bytecode
{
    public struct Boolean : ICombatByte
    {
        public readonly bool Value;

        public Boolean(bool value)
        {
            Value = value;
        }
    }
}