using System;
using System.Collections;
using System.Collections.Generic;
using Utils.Extensions;

namespace Tooling.StaticData.Bytecode
{
    public class AssignVariableModel : IInstructionModel
    {
        public string Name;
        public readonly ValueModel Value = new();
    }

    public class ValueModel
    {
        public System.Type Type;
        public Source Source;
        public GameFunction GameFunction;

        public string String;
        public bool Bool;
        public long Long;
        public double Double;
        public ListValueModel List;
    }

    public class ListValueModel : IList, IEnumerable<ValueModel>
    {
        public System.Type Type = typeof(Null);

        private readonly List<ValueModel> list = new();

        #region IEnumerable

        IEnumerator<ValueModel> IEnumerable<ValueModel>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IList

        public int Count => list.Count;
        public bool IsSynchronized => ((IList)list).IsSynchronized;
        public object SyncRoot => ((IList)list).SyncRoot;
        public bool IsFixedSize => ((IList)list).IsFixedSize;
        public bool IsReadOnly => ((IList)list).IsReadOnly;
        public IEnumerator GetEnumerator() => list.GetEnumerator();
        public void CopyTo(Array array, int index) => list.CopyTo((ValueModel[])array, index);
        public int Add(object value) => ((IList)list).Add((ValueModel)value);
        public void Clear() => list.Clear();
        public bool Contains(object value) => list.Contains((ValueModel)value);
        public int IndexOf(object value) => list.IndexOf((ValueModel)value);
        public void Insert(int index, object value) => list.Insert(index, (ValueModel)value);
        public void Remove(object value) => list.Remove((ValueModel)value);
        public void RemoveAt(int index) => list.RemoveAt(index);

        public object this[int index]
        {
            get => list[index];
            set => list[index] = (ValueModel)value;
        }

        #endregion
    }

    public class ReadVariableModel : IInstructionModel
    {
        public string Name;
    }

    public enum Source
    {
        Manual,
        GameFunction,
        StaticData
    }

    public enum GameFunction
    {
        GetTargetedCombatParticipant,
        GetSelf,
        GetRandom,
        GetAllCombatParticipants,
        GetStat,
        GetBuff
    }

    public enum Type
    {
        Null,
        Bool,
        String,
        Int,
        Long,
        Float,
        Double,
        List,
        CombatParticipant
    }

    public interface IValueType
    {
    }

    public struct Null : IValueType
    {
    }

    public struct Bool : IValueType
    {
    }

    public struct String : IValueType
    {
    }

    public struct Int : IValueType
    {
    }

    public struct Long : IValueType
    {
    }

    public struct Float : IValueType
    {
    }

    public struct Double : IValueType
    {
    }

    public struct CombatParticipant : IValueType
    {
    }

    public struct ValueList<T> : IValueType where T : IValueType
    {
    }
}