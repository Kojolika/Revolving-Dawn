using Fight.Engine.Bytecode;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tooling.StaticData
{
    [System.Serializable]
    public class Statement : StaticData, ITriggerPoint
    {
        public Fight.Engine.Bytecode.Statement ByteStatement;

        public void Execute(Context context)
        {
            ByteStatement?.Execute(context);
        }
    }
}