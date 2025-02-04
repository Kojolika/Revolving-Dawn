using Newtonsoft.Json;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct PushBoolean : IInstruction
    {
        [SerializeField, JsonProperty]
        private bool value;

        public PushBoolean(bool value)
        {
            this.value = value;
        }

        public void Execute(Context context)
        {
            context.Memory.Push(new Boolean(value));

            context.Logger.Log(LogLevel.Info, $"Push Boolean {value}");
        }
    }
}