using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct PushLiteral : IInstruction
    {
        [SerializeField, JsonProperty]
        private float value;

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