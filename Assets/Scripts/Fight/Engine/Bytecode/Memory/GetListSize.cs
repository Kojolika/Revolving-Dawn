using System.Collections;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Gets the number size of the list on top of the stack.
    /// </summary>
    [System.Serializable]
    public struct GetListSize : IPush<Literal>
    {
        public void Execute(Context context)
        {
            var stackTop = context.Memory.Peek();
            if (stackTop is IList list)
            {
                var listSize = list.Count;
                context.Memory.Push(new Literal(listSize));
                context.Logger.Log(LogLevel.Info, $"Pushed size of list : {listSize}");
            }
            else
            {
                context.Logger.Log(LogLevel.Error, $"Top of stack is not a {typeof(StoreableList<>)}!");
            }
        }
    }
}