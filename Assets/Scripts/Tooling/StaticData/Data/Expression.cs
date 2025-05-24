using Newtonsoft.Json;
using UnityEngine;
using Tooling.StaticData.Bytecode;

namespace Tooling.StaticData
{
    [System.Serializable]
    public class Expression : StaticData
    {
        [SerializeField, JsonProperty]
        private ExpressionInstruction expressionInstruction;
    }
}