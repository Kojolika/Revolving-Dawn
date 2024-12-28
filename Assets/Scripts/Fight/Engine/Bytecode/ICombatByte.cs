// ReSharper disable UnusedTypeParameter

using System;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// We're using the bytecode pattern for our games combat.
    /// </summary>
    public interface ICombatByte
    {
    }

    /// <typeparam name="T">Type that is on the popped off the stack prior to this byte</typeparam>
    public interface IPopByte<in T> : ICombatByte
        where T : ICombatByte
    {
        void Pop(T input);
    }

    /// <typeparam name="T1">Type that is on the popped off the stack prior to this byte</typeparam>
    /// <typeparam name="T2">Type that is on the popped off the stack prior to this byte</typeparam>
    public interface IPopByte<in T1, in T2> : ICombatByte
        where T1 : ICombatByte
        where T2 : ICombatByte
    {
        void Pop(T1 input, T2 input2);
    }

    /// <typeparam name="T1">Type that is on the popped off the stack prior to this byte</typeparam>
    /// <typeparam name="T2">Type that is on the popped off the stack prior to this byte</typeparam>
    /// <typeparam name="T3">Type that is on the popped off the stack prior to this byte</typeparam>
    public interface IPopByte<in T1, in T2, in T3> : ICombatByte
        where T1 : ICombatByte
        where T2 : ICombatByte
        where T3 : ICombatByte
    {
        void Pop(T1 input, T2 input2, T3 input3);
    }

    /// <typeparam name="T">Type that is on the pushed on the stack after this byte</typeparam>
    public interface IPushByte<out T> : ICombatByte
        where T : ICombatByte
    {
        T Push();
    }

    /// <typeparam name="T1">Type that is on the pushed on the stack after this byte</typeparam>
    /// <typeparam name="T2">Type that is on the pushed on the stack after this byte</typeparam>
    public interface IPushByte<T1, T2> : ICombatByte
        where T1 : ICombatByte
        where T2 : ICombatByte
    {
        (T1, T2) Push();
    }

    /// <typeparam name="T1">Type that is on the pushed on the stack after this byte</typeparam>
    /// <typeparam name="T2">Type that is on the pushed on the stack after this byte</typeparam>
    /// <typeparam name="T3">Type that is on the pushed on the stack after this byte</typeparam>
    public interface IPushByte<T1, T2, T3> : ICombatByte
        where T1 : ICombatByte
        where T2 : ICombatByte
        where T3 : ICombatByte
    {
        (T1, T2, T3) Push();
    }
}