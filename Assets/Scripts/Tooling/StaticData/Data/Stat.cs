using Tooling.StaticData.Bytecode;

namespace Tooling.StaticData
{
    [ByteObject]
    public class Stat : StaticData
    {
        public bool   IsInternal;
        public LocKey LocName;
        public LocKey Description;
    }
}