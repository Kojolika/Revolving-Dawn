using UnityEngine;

namespace Fight.Engine.Bytecode
{
    public struct Boolean : ICombatByte
    {
        [SerializeField] private bool value;

        public bool Value => value;

        public Boolean(bool value)
        {
            this.value = value;
        }

        public string Log()
        {
            return Value.ToString();
        }
    }
}