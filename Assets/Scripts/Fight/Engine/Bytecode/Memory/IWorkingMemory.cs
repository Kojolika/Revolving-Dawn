using System.Collections.Generic;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Holds the memory that each bytecode instruction has access to. For our implementation, the only memory we have avaiable is stack
    /// </summary>
    public interface IWorkingMemory
    {
        void Push(IStoreable value);
        IStoreable Pop();

        /// <summary>
        /// Tries to pop a specific type off of the stack.
        /// </summary>
        /// <param name="value">the returned instruction of type <see cref="T"/></param>
        /// <returns>true if succeeded</returns>
        bool TryPop<T>(out T value) where T : IStoreable;

        /// <summary>
        /// Tries to pop 2 specific instructions off of the stack.
        /// This does not care about the order of the 2, just that both were on top of the stack.
        /// </summary>
        /// <param name="value">the returned instruction of type <see cref="T1"/></param>
        /// <param name="value2">the returned instruction of type <see cref="T2"/></param>
        /// <returns>true if succeeded</returns>
        bool TryPop<T1, T2>(out T1 value, out T2 value2)
            where T1 : IStoreable where T2 : IStoreable;
    }

    public class WorkingMemory : IWorkingMemory
    {
        private readonly Stack<IStoreable> stack = new();

        public void Push(IStoreable value)
        {
            stack.Push(value);
        }

        public IStoreable Pop()
        {
            return stack.Pop();
        }

        public bool TryPop<T>(out T value)
            where T : IStoreable
        {
            if (stack.TryPop(out var nextValue)
                && nextValue is T nextValueT)
            {
                value = nextValueT;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryPop<T1, T2>(out T1 value, out T2 value2)
            where T1 : IStoreable where T2 : IStoreable
        {
            value = default;
            value2 = default;

            if (!stack.TryPop(out var nextValue))
            {
                return false;
            }

            bool foundFirstValue = false;
            bool foundSecondValue = false;

            switch (nextValue)
            {
                case T1 nextValueT:
                    foundFirstValue = true;
                    value = nextValueT;
                    break;
                case T2 nextValueT2:
                    foundSecondValue = true;
                    value2 = nextValueT2;
                    break;
            }

            if (!stack.TryPop(out nextValue))
            {
                return false;
            }

            switch (nextValue)
            {
                case T1 nextValueT:
                    foundFirstValue = true;
                    value = nextValueT;
                    break;
                case T2 nextValueT2:
                    foundSecondValue = true;
                    value2 = nextValueT2;
                    break;
            }

            return foundFirstValue && foundSecondValue;
        }
    }

    public class MockWorkingMemory : IWorkingMemory
    {
        private readonly Stack<IStoreable> stack = new();

        public void Push(IStoreable value)
        {
            stack.Push(value);
        }

        public IStoreable Pop()
        {
            return stack.Pop();
        }

        public bool TryPop<T>(out T value)
            where T : IStoreable
        {
            if (stack.TryPop(out var nextValue)
                && nextValue is T nextValueT)
            {
                value = nextValueT;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryPop<T1, T2>(out T1 value, out T2 value2)
            where T1 : IStoreable where T2 : IStoreable
        {
            value = default;
            value2 = default;

            if (!stack.TryPop(out var nextValue))
            {
                return false;
            }

            bool foundFirstValue = false;
            bool foundSecondValue = false;

            switch (nextValue)
            {
                case T1 nextValueT:
                    foundFirstValue = true;
                    value = nextValueT;
                    break;
                case T2 nextValueT2:
                    foundSecondValue = true;
                    value2 = nextValueT2;
                    break;
            }

            if (!stack.TryPop(out nextValue))
            {
                return false;
            }

            switch (nextValue)
            {
                case T1 nextValueT:
                    foundFirstValue = true;
                    value = nextValueT;
                    break;
                case T2 nextValueT2:
                    foundSecondValue = true;
                    value2 = nextValueT2;
                    break;
            }

            return foundFirstValue && foundSecondValue;
        }
    }
}