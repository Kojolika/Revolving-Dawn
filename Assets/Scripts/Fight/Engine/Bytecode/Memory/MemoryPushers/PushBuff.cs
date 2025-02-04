using Newtonsoft.Json;
using Tooling.StaticData;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct PushBuff : IInstruction
    {
        [SerializeField, JsonProperty]
        private Buff value;

        public PushBuff(Buff value)
        {
            this.value = value;
        }

        public void Execute(Context context)
        {
            context.Memory.Push(value);

            context.Logger.Log(LogLevel.Info, $"Push {nameof(Buff)} {value}");
        }
    }
}