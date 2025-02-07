using System.Collections.Generic;
using Newtonsoft.Json;
using Tooling.StaticData;
using Tooling.StaticData.Attributes;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    [GeneralFieldIgnore(IgnoreType.Interface)]
    public class Statement : IInstruction
    {
        [SerializeField, JsonProperty] private List<IInstruction> instructions;

        public void Execute(Context context)
        {
            foreach (var instruction in instructions)
            {
                instruction.Execute(context);
            }
        }
    }
}