using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Tooling.StaticData.EditorUI;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    [GeneralFieldIgnore(IgnoreType.Interface)]
    public class Statement : IInstruction
    {
        public List<IInstruction> Instructions;

        public void Execute(Context context)
        {
            foreach (var instruction in Instructions)
            {
                instruction.Execute(context);
            }
        }
    }
}