using Fight.Engine.Bytecode;
using Newtonsoft.Json;
using UnityEngine;

namespace Tooling.StaticData
{
    [System.Serializable]
    public class Expression : StaticData, IInstruction
    {
        [SerializeField, JsonProperty] 
        private Fight.Engine.Bytecode.Expression expression;

        public void Execute(Context context)
        {
            expression?.Execute(context);
        }
    }
}