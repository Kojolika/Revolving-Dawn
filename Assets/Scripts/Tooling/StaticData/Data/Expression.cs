using Newtonsoft.Json;
using UnityEngine;

namespace Tooling.StaticData
{
    [System.Serializable]
    public class Expression : StaticData, IInstruction
    {
        [SerializeField, JsonProperty]
        private ExpressionInstruction expressionInstruction;
    }
}