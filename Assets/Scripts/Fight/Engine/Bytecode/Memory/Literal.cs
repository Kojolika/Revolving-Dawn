using Newtonsoft.Json;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct Literal : IStoreable
    {
        [SerializeField, JsonProperty]
        private float value;

        [JsonIgnore]
        public float Value => value;

        public Literal(float value)
        {
            this.value = value;
        }
    }
}