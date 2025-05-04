using Newtonsoft.Json;
using UnityEngine;
using Tooling.StaticData.Bytecode;

namespace Tooling.StaticData
{
    [System.Serializable]
    public class Expression : StaticData, IInstructionModel
    {
        [SerializeField, JsonProperty]
        private ExpressionInstruction expressionInstruction;
    }
}