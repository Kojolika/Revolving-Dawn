using System.Globalization;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    public struct Literal : ICombatByte
    {
        [SerializeField] private float value;

        public float Value => value;

        public Literal(float value)
        {
            this.value = value;
        }

        public string Log()
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}