using UnityEngine;

namespace Fight.Engine.Bytecode
{
    public struct PushBoolean : IInstruction
    {
        [SerializeField] private bool value;

        public bool Value => value;

        public PushBoolean(bool value)
        {
            this.value = value;
        }

        public void Execute(Context context)
        {
            context.Memory.Push(new Boolean(value));

            context.Logger.Log(LogLevel.Info, $"Push Boolean {value}");
        }
    }
}