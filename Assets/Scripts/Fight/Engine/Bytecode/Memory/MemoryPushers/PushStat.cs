using Newtonsoft.Json;
using Tooling.StaticData;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct PushStat : IInstruction
    {
        [SerializeField, JsonProperty]
        private Stat value;

        public PushStat(Stat value)
        {
            this.value = value;
        }

        public void Execute(Context context)
        {
            context.Memory.Push(value);

            context.Logger.Log(LogLevel.Info, $"Push {nameof(Stat)} {value}");
        }
    }
}