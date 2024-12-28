namespace Fight.Engine.Bytecode
{
    public struct Or :
        IPopByte<Boolean, Boolean>,
        IPushByte<Boolean>
    {
        private Boolean result;

        public void Pop(Boolean input, Boolean input2)
            => result = new Boolean(input.Value || input2.Value);

        public Boolean Push() => result;
    }
}