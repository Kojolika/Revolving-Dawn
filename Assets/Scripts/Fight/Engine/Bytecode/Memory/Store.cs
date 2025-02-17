using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct Store : IPush
    {
        [SerializeField, JsonProperty] private IStoreable storeable;

        public Type Type => storeable?.GetType();

        public void Execute(Context context)
        {
            context.Memory.Push(storeable);

            context.Logger.Log(LogLevel.Info, $"Pushed {storeable}");
        }
    }
}