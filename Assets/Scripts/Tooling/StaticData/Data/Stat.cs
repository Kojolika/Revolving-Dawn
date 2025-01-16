using Fight.Engine.Bytecode;

namespace Tooling.StaticData
{
    public class Stat : StaticData, ICombatByte
    {
        public LocKey LocName;
        public LocKey Description;

        public string Log()
        {
            return Name;
        }
    }
}