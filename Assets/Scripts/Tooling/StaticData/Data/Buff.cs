using System.Collections.Generic;
using Tooling.StaticData.Bytecode;

namespace Tooling.StaticData
{
    [ByteObject]
    public class Buff : StaticData
    {
        [ByteProperty(Type.Bool)]
        public bool IsStackable;

        [ByteProperty(Type.Long)]
        public long MaxStackSize;
    }
}