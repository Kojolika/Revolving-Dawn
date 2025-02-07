using Newtonsoft.Json;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct Store : IInstruction
    {
        [SerializeField, JsonProperty] private IStoreable storeable;

        public void Execute(Context context)
        {
            context.Memory.Push(storeable);

            context.Logger.Log(LogLevel.Info, $"Pushed {storeable}");
        }
    }
}