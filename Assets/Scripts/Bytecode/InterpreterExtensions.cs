namespace Bytecode
{
    public static class InterpreterExtensions
    {
/*         public static bool TryGetValue<T, V>(this IByteCodeInterpreter<T> byteCodeInterpreter, out V value) where T : struct
        {
            if (byteCodeInterpreter != null
                && byteCodeInterpreter.CurrentInstructions.TryPop(out var instruction)
                && instruction.Value is V val)
            {
                value = val;
                return true;
            }
            value = default;
            return false;
        }

        public static bool TryGetValue<TInstruction, TValue, TParam>(this IByteCodeInterpreter<TInstruction, TParam> byteCodeInterpreter,
            out TValue value) where TInstruction : struct
        {
            if (byteCodeInterpreter != null
                && byteCodeInterpreter.CurrentInstructions.TryPop(out var instruction)
                && instruction.Value is TValue val)
            {
                value = val;
                return true;
            }
            value = default;
            return false;
        } */
    }
}