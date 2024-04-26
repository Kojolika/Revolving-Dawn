using UnityEngine;

namespace Models.Buffs
{
    public abstract class BuffDefinition : ScriptableObject, IBuffDefinition
    {
        public abstract string Name { get; }
    }
}