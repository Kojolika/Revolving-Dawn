using UnityEngine;

namespace Fight.Engine.Bytecode
{
    public struct Literal : IStoreable
    {
        [SerializeField] private float value;

        public float Value => value;

        public Literal(float value)
        {
            this.value = value;
        }
    }
}