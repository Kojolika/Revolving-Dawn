using System.Globalization;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct PushLiteral : IInstruction
    {
        [SerializeField] private float value;

        public float Value => value;

        public PushLiteral(float value)
        {
            this.value = value;
        }

        public void Execute(Context context)
        {
            context.Memory.Push(new Literal(value));
            context.Logger.Log(LogLevel.Info, value.ToString(CultureInfo.InvariantCulture));
        }
    }
}