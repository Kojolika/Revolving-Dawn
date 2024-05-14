using UnityEngine;

namespace Models
{
    public abstract class RuntimeModel<D>
    {
        public abstract D Definition { get; }
    }
}