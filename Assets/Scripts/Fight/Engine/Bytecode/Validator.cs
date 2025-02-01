using System.Collections.Generic;

namespace Fight.Engine.Bytecode
{
    public static class Validator
    {
        /*public static ValidationResult ValidatePop<T>(IPop<T> iPopToValidate, IInstruction instruction, out T successResult)
            where T : IInstruction
        {
            var isValidType = instruction is IReduceTo<T> or T;
            if (!isValidType)
            {
                successResult = default;
                return new ValidationResult
                {
                    IsSuccess = false,
                    Errors = new List<string>
                    {
                        $"Byte  popped off the stack does not match the corresponding type for {iPopToValidate.GetType()}" +
                        $"Expected: {typeof(T)}" +
                        $"first={instruction?.GetType()},"
                    }
                };
            }

            var poppedByte = instruction is IReduceTo<T> nextByte
                ? nextByte.Reduce()
                : (T)instruction;

            successResult = poppedByte;

            return new ValidationResult { IsSuccess = true };
        }

        /// <summary>
        /// Validates whether the given bytes that are popped off the stack correspond to the correct types that
        /// the <see cref="iPopToValidate"/> byte requires.
        /// Essentially makes sure <see cref="combatByte1"/> and <see cref="combatByte2"/>
        /// match the given input parameters for the given <see cref="IPop"/>
        /// </summary>
        public static ValidationResult ValidatePop<T1, T2>(
            IPop<T1, T2> iPopToValidate,
            IInstruction combatByte1,
            IInstruction combatByte2,
            out T1 successResult1,
            out T2 successResult2
        )
            where T1 : IInstruction
            where T2 : IInstruction
        {
            successResult1 = default;
            successResult2 = default;

            bool encounteredError = false;
            List<string> errors = null;

            if (TryGetArg(combatByte1, out T1 t1Arg))
            {
                successResult1 = t1Arg;
            }
            else if (TryGetArg(combatByte2, out T1 t1Arg2))
            {
                successResult1 = t1Arg2;
            }
            else
            {
                var errorMessage =
                    $"Neither bytes that were popped off the stack match the corresponding types for {iPopToValidate.GetType()} " +
                    $"Expected: {typeof(T1)} and {typeof(T2)} " +
                    $"first={combatByte1?.GetType()}, second={combatByte2?.GetType()}";

                encounteredError = true;
                errors = new List<string> { errorMessage };
            }

            if (TryGetArg(combatByte1, out T2 t2Arg))
            {
                successResult2 = t2Arg;
            }
            else if (TryGetArg(combatByte2, out T2 t2Arg2))
            {
                successResult2 = t2Arg2;
            }
            else
            {
                var errorMessage =
                    $"Neither bytes that were popped off the stack match the corresponding types for {iPopToValidate.GetType()} " +
                    $"Expected: {typeof(T1)} and {typeof(T2)} " +
                    $"first={combatByte1?.GetType()}, second={combatByte2?.GetType()}";

                encounteredError = true;
                errors ??= new List<string>();
                errors.Add(errorMessage);
            }

            return new ValidationResult
            {
                IsSuccess = !encounteredError,
                Errors = errors
            };

            bool TryGetArg<T>(IInstruction cByte, out T arg) where T : IInstruction
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
        }*/
    }
}