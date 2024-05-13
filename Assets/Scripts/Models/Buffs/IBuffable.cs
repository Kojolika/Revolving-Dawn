using System.Collections.Generic;

namespace Models.Buffs
{
    public interface IBuffable
    {
        List<Buff> Buffs { get; }
    }
}