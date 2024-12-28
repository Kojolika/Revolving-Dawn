namespace Fight.Engine.Bytecode
{
    public struct Literal : ICombatByte
    {
        public readonly float Value;

        public Literal(float value)
        {
            Value = value;
        }
    }
}