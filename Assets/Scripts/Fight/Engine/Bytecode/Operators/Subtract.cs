namespace Fight.Engine.Bytecode
{
    public struct Subtract :
        IPopByte<Literal, Literal>,
        IPushByte<Literal>
    {
        private Literal value;

        public void Pop(Literal input, Literal input2)
        {
            value = new Literal(input.Value - input2.Value);
        }

        public Literal Push() => value;
    }
}