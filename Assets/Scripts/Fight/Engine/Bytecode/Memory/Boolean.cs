using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [Serializable]
    public struct Boolean : IStoreable
    {
        [SerializeField, JsonProperty] public bool value;

        public Boolean(bool value)
        {
            this.value = value;
        }
    }
}