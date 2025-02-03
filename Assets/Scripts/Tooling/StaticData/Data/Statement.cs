using Fight.Engine.Bytecode;
using Newtonsoft.Json;
using UnityEngine;

namespace Tooling.StaticData
{
    [System.Serializable]
    public class Statement : StaticData, ITriggerPoint
    {
        [SerializeField, JsonProperty]
        private Fight.Engine.Bytecode.Statement statement;

        public void Execute(Context context)
        {
            statement?.Execute(context);
        }
    }
}