using System.Collections.Generic;
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

        ValidationResult OnBytesPopped(params ICombatByte[] poppedBytes);
    }

    /// <typeparam name="T">Type that is on the popped off the stack prior to this byte</typeparam>
    public interface IPop<in T> : IPop
        where T : ICombatByte
    {
        int IPop.Amount => 1;

        ValidationResult IPop.OnBytesPopped(params ICombatByte[] poppedBytes)
        {
            var validationResult = Validator.ValidatePop(this, poppedBytes[0], out var successResult);

            if (validationResult.IsSuccess)
            {
                OnBytesPopped(successResult);
            }

            return validationResult;
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

        ValidationResult IPop.OnBytesPopped(params ICombatByte[] poppedBytes)
        {
            var validationResult = Validator.ValidatePop(this,
                poppedBytes[0],
                poppedBytes[1],
                out var successResult1,
                out var successResult2
            );

            if (validationResult.IsSuccess)
            {
                OnBytesPopped(successResult1, successResult2);
            }

            return validationResult;
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