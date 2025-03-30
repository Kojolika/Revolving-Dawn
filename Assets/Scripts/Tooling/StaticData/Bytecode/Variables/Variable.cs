using System.Collections.Generic;

namespace Tooling.StaticData.Bytecode
{
    public abstract class Variable<T> : IVariable
    {
        public string Name;
        public abstract Type Type { get; }
        public abstract T Value { get; set; }

        public abstract bool IsComputedAtRuntime { get; }

        string IVariable.Name => Name;
    }

    public abstract class VariableList<T> : Variable<List<T>>
    {
    }
}