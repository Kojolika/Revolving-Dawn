namespace Fight.Engine.Bytecode
{
    public struct And : IPop<Boolean, Boolean>, IReduceTo<Boolean>
    {
        private Boolean result;

        public void OnBytesPopped(Boolean input1, Boolean input2)
            => result = new Boolean(input1.Value && input2.Value);

        public Boolean Reduce() => result;

        public string Log()
        {
            return result.Log();
        }
    }
}