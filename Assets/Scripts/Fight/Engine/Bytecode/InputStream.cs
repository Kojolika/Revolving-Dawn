using System.Collections.Generic;

namespace Fight.Engine.Bytecode
{
    public interface IInputStream
    {
        /// <summary>
        /// Reads the next instruction in the input
        /// </summary>
        bool TryReadNext<T>(out T instruction) where T : IInstruction;

        /// <summary>
        /// Reads the next two instructions in the input. Use this to disregard the order.
        /// </summary>
        bool TryReadNextTwo<T1, T2>(out T1 instruction, out T2 instruction2) where T1 : IInstruction where T2 : IInstruction;
    }

    public class InputStream : IInputStream
    {
        private readonly List<IInstruction> input;
        private readonly int instructionCount;
        private int position;

        public InputStream(List<IInstruction> input)
        {
            this.input = input;
            instructionCount = input.Count;
        }

        public bool TryReadNext<T>(out T instruction) where T : IInstruction
        {
            if (ReadNext(out var nextInstruction) && nextInstruction is T instructionT)
            {
                instruction = instructionT;
                return true;
            }

            instruction = default;
            return false;
        }

        public bool TryReadNextTwo<T1, T2>(out T1 instruction, out T2 instruction2)
            where T1 : IInstruction where T2 : IInstruction

        {
            instruction = default;
            instruction2 = default;

            if (!ReadNext(out var nextInstruction))
            {
                return false;
            }

            bool foundFirstValue = false;
            bool foundSecondValue = false;

            switch (nextInstruction)
            {
                case T1 nextValueT:
                    foundFirstValue = true;
                    instruction = nextValueT;
                    break;
                case T2 nextValueT2:
                    foundSecondValue = true;
                    instruction2 = nextValueT2;
                    break;
            }

            if (!ReadNext(out nextInstruction))
            {
                return false;
            }

            switch (nextInstruction)
            {
                case T1 nextValueT:
                    foundFirstValue = true;
                    instruction = nextValueT;
                    break;
                case T2 nextValueT2:
                    foundSecondValue = true;
                    instruction2 = nextValueT2;
                    break;
            }

            return foundFirstValue && foundSecondValue;
        }

        private bool ReadNext(out IInstruction instruction)
        {
            position++;
            if (position >= instructionCount)
            {
                instruction = default;
                return false;
            }

            instruction = input[position];
            return true;
        }
    }
}