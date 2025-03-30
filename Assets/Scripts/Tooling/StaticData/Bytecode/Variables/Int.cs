using Tooling.StaticData.EditorUI;
using UnityEngine;

namespace Tooling.StaticData.Bytecode
{
    [DisplayName("Primitive/Int")]
    public class Int : Variable<int>
    {
        public override Type Type => Type.Int;
        public override bool IsComputedAtRuntime => false;

        public override int Value
        {
            get => value;
            set => this.value = value;
        }

        [SerializeField]
        private int value;
    }
}