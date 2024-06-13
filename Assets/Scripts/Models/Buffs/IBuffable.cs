using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Models.Buffs
{
    public interface IBuffable
    {
        BuffList Buffs { get; }
    }
}
