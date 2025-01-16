namespace Fight.Engine.Bytecode
{
    public struct Add :
        IPop<Literal, Literal>,
        IReduceTo<Literal>
    {
        private Literal value;

        public void OnBytesPopped(Literal input1, Literal input2)
        {
            value = new Literal(input1.Value + input2.Value);
        }

        public Literal Reduce() => value;

        public string Log()
        {
            return value.Log();
        }
    }
}