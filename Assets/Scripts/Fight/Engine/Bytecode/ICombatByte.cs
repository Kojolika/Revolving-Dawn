using Tooling.Logging;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// We're using the bytecode pattern for our games combat.
    /// </summary>
    public interface ICombatByte
    {
        // TODO: Loc
        string Log();
    }


    public interface IPop : ICombatByte
    {
        /// <summary>
        /// Specifies how many <see cref="ICombatByte"/>s should be popped off the stack
        /// and passed into <see cref="OnBytesPopped"/>
        /// </summary>
        int Amount { get; }

        void OnBytesPopped(params ICombatByte[] poppedBytes);
    }

    /// <typeparam name="T">Type that is on the popped off the stack prior to this byte</typeparam>
    public interface IPop<in T> : IPop
        where T : ICombatByte
    {
        int IPop.Amount => 1;

        void IPop.OnBytesPopped(params ICombatByte[] poppedBytes)
        {
            var first = poppedBytes[0];

            OnBytesPopped(first is IReduceTo<T> nextByte
                ? nextByte.Reduce()
                : (T)first
            );
        }

        void OnBytesPopped(T input);
    }

    /// <typeparam name="T1">Type that is on the popped off the stack prior to this byte</typeparam>
    /// <typeparam name="T2">Type that is on the popped off the stack prior to this byte</typeparam>
    public interface IPop<in T1, in T2> : IPop
        where T1 : ICombatByte
        where T2 : ICombatByte
    {
        int IPop.Amount => 2;

        void IPop.OnBytesPopped(params ICombatByte[] poppedBytes)
        {
            var first = poppedBytes?[0];
            var second = poppedBytes?[1];

            T1 firstArg;
            T2 secondArg;

            if (TryGetArg(first, out T1 t1Arg))
            {
                firstArg = t1Arg;
            }
            else if (TryGetArg(second, out T1 t1Arg2))
            {
                firstArg = t1Arg2;
            }
            else
            {
                MyLogger.LogError($"Neither bytes that were popped off the stack match the corresponding types for {GetType()}" +
                                  $"Expected: {typeof(T1)} and {typeof(T2)}" +
                                  $"first={first?.GetType()}, second={second?.GetType()}");
                return;
            }

            if (TryGetArg(first, out T2 t2Arg))
            {
                secondArg = t2Arg;
            }
            else if (TryGetArg(second, out T2 t2Arg2))
            {
                secondArg = t2Arg2;
            }
            else
            {
                MyLogger.LogError($"Neither bytes that were popped off the stack match the corresponding types for {GetType()}" +
                                  $"Expected: {typeof(T1)} and {typeof(T2)}" +
                                  $"first={first?.GetType()}, second={second?.GetType()}");
                return;
            }


            OnBytesPopped(firstArg, secondArg);

            return;

            bool TryGetArg<T>(ICombatByte cByte, out T arg) where T : ICombatByte
            {
                switch (cByte)
                {
                    case T tArg:
                        arg = tArg;
                        return true;
                    case IReduceTo<T> reducer:
                        arg = reducer.Reduce();
                        return true;
                    default:
                        arg = default;
                        return false;
                }
            }
        }

        void OnBytesPopped(T1 input1, T2 input2);
    }

    /// <summary>
    /// Replaces this byte with type <see cref="T"/> when this byte is popped
    /// off of the stack. Use this to grab objects during runtime that are not available at compile time.
    /// (Like the character that the player is targeting!)
    /// </summary>
    /// <typeparam name="T">Type that replaces the byte</typeparam>
    public interface IReduceTo<out T> : ICombatByte
        where T : ICombatByte
    {
        T Reduce();
    }

    /// <typeparam name="T1">Type that is on the pushed on the stack after this byte</typeparam>
    /// <typeparam name="T2">Type that is on the pushed on the stack after this byte</typeparam>
    public interface IPush<T1, T2> : ICombatByte
        where T1 : ICombatByte
        where T2 : ICombatByte
    {
        (T1, T2) Push();
    }

    /// <typeparam name="T1">Type that is on the pushed on the stack after this byte</typeparam>
    /// <typeparam name="T2">Type that is on the pushed on the stack after this byte</typeparam>
    /// <typeparam name="T3">Type that is on the pushed on the stack after this byte</typeparam>
    public interface IPush<T1, T2, T3> : ICombatByte
        where T1 : ICombatByte
        where T2 : ICombatByte
        where T3 : ICombatByte
    {
        (T1, T2, T3) Push();
    }
}