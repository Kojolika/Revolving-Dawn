using System.Collections.Generic;

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

        public class ListValueModel
        {
            public System.Type Type;
            public List<ValueModel> List;
        }
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