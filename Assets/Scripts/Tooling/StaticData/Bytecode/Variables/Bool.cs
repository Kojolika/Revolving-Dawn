using Tooling.StaticData.EditorUI;
using UnityEngine;

namespace Tooling.StaticData.Bytecode
{
    [DisplayName("Primitive/Bool")]
    public class Bool : Variable<bool>
    {
        public override Type Type => Type.Bool;
        public override bool IsComputedAtRuntime => false;

        public override bool Value
        {
            get => value;
            set => this.value = value;
        }

        [SerializeField]
        private bool value;
    }
}