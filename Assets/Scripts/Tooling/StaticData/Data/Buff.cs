using System.Collections.Generic;
using Tooling.StaticData.Bytecode;

namespace Tooling.StaticData
{
    [Object]
    public class Buff : StaticData
    {
        [Property(Type.Bool)]
        public bool IsStackable;

        [Property(Type.Long)]
        public long MaxStackSize;
    }
}