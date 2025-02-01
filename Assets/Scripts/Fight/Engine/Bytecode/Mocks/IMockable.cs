using Tooling.StaticData.Attributes;

namespace Fight.Engine.Bytecode
{
    public interface IMockable : IInstruction
    {
    }

    [GeneralFieldIgnore]
    public interface IMock : IInstruction
    {
    }
}