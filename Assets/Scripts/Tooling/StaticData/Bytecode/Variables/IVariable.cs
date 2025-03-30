namespace Tooling.StaticData.Bytecode
{
    public interface IVariable
    {
        public string Name { get; }
        public Type Type { get; }
        public bool IsComputedAtRuntime { get; }
    }
}