using System.Collections.Generic;
using Newtonsoft.Json;
using Tooling.StaticData.EditorUI;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    [GeneralFieldIgnore(IgnoreType.Interface)]
    public class Expression : IInstruction
    {
        [SerializeField, JsonProperty] private List<IInstruction> instructions;

        private IStoreable result;
        private bool hasBeenEvaluated;

        /// <summary>
        /// Returns what the expression result is (<see cref="Boolean"/>, <see cref="Literal"/>... etc.)
        /// </summary>
        public bool TryEvaluate<T>(out T result) where T : IStoreable
        {
            result = default;
            if (!hasBeenEvaluated || this.result is not T resultT)
            {
                return false;
            }

            result = resultT;
            return true;
        }

        public void Execute(Context context)
        {
            foreach (var instruction in instructions)
            {
                instruction.Execute(context);
            }

            if (context.Memory.TryPop(out result))
            {
                hasBeenEvaluated = true;
            }
            else
            {
                context.Logger.Log(LogLevel.Error, "Expected value on top of stack from expression!");
            }
        }
    }
}