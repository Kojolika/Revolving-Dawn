using Tooling.StaticData.EditorUI.Bytecode;

namespace Tooling.StaticData.EditorUI
{
    [ByteObject]
    public class Stat : StaticData
    {
        public bool   IsInternal;
        public LocKey LocName;
        public LocKey Description;
    }
}