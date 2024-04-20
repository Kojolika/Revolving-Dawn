using System.Collections.Generic;

namespace Models.Buffs
{
    public interface IBuffable
    {
        List<IBuff> Buffs { get; }
    }
}