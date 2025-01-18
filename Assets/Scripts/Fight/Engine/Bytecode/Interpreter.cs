using System;
using System.Collections.Generic;
using Tooling.Logging;
using Utils.Extensions;

namespace Fight.Engine.Bytecode
{
    public class Interpreter
    {
        public static void Interpret(Stack<ICombatByte> instructions)
        {
            while (!instructions.IsNullOrEmpty())
            {
                var nextInstruction = instructions.Pop();

                if (nextInstruction is IPop iPop)
                {
                    var poppedBytes = new ICombatByte[iPop.Amount];
                    for (int i = 0; i < iPop.Amount; i++)
                    {
                        poppedBytes[i] = instructions.Pop();
                        MyLogger.Log(poppedBytes[i].Log());
                    }

                    iPop.OnBytesPopped(poppedBytes);
                }

                MyLogger.Log(nextInstruction.Log());
            }
        }

        public ValidationResult ValidateWithMocks(Stack<ICombatByte> instructions)
        {
            var validationResult = new ValidationResult
            {
                Errors = new List<string>()
            };

            while (!instructions.IsNullOrEmpty())
            {
                var nextInstruction = instructions.Pop();
                MyLogger.Log($"Popping type: {nextInstruction.GetType()}");

                if (ShouldMock(nextInstruction))
                {
                    nextInstruction = GetMockable(nextInstruction);
                }

                if (nextInstruction is IPop iPop)
                {
                    var poppedBytes = new ICombatByte[iPop.Amount];
                    for (int i = 0; i < iPop.Amount; i++)
                    {
                        if (!instructions.TryPop(out var poppedInstruction))
                        {
                            validationResult.IsSuccess = false;
                            validationResult.Errors.Add($"Could not pop another instruction for {iPop.GetType()} " +
                                                        $"Expected {iPop.Amount} total pops, but failed at pop {i}");

                            continue;
                        }

                        MyLogger.Log($"Popping type: {poppedInstruction.GetType()}");

                        poppedBytes[i] = ShouldMock(poppedInstruction)
                            ? GetMockable(poppedInstruction)
                            : poppedInstruction;

                        MyLogger.Log($"{poppedBytes[i].GetType()}: {poppedBytes[i].Log()}");
                    }

                    var popValidationResult = iPop.OnBytesPopped(poppedBytes);
                    validationResult.IsSuccess &= popValidationResult.IsSuccess;

                    if (!popValidationResult.Errors.IsNullOrEmpty())
                    {
                        validationResult.Errors.AddRange(popValidationResult.Errors);
                    }
                }

                MyLogger.Log(nextInstruction.Log());
            }

            return validationResult;
        }

        private Dictionary<Type, IMock> mocks = new();

        public void RegisterMockable<TMockable, TMock>()
            where TMockable : IMockable
            where TMock : IMock, new()
        {
            mocks[typeof(TMockable)] = new TMock();
        }

        private bool ShouldMock(ICombatByte combatByte)
        {
            return combatByte is IMockable mockable
                   && mocks.ContainsKey(mockable.GetType());
        }

        private ICombatByte GetMockable(ICombatByte combatByte)
        {
            return mocks[combatByte.GetType()];
        }
    }
}