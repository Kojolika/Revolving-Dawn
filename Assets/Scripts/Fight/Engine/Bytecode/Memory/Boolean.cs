namespace Fight.Engine.Bytecode
{
    public struct Boolean : IStoreable
    {
        public readonly bool Value;

        public Boolean(bool value)
        {
            Value = value;
        }
    }
}