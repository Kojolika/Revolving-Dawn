using UnityEngine;

namespace Models
{
    public abstract class RuntimeModel<D> where D : ScriptableObject
    {
        public abstract D Definition { get; }
    }
}