using System.Collections.Generic;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Specifies this type can be stored on the <see cref="IWorkingMemory"/> stack.
    /// </summary>
    public interface IStoreable
    {
    }

    public class StoreableList<T> : List<T>, IStoreable
        where T : IStoreable
    {
    }
}