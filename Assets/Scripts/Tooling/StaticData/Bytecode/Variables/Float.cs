using Newtonsoft.Json;
using Tooling.StaticData.EditorUI;
using UnityEngine;

namespace Tooling.StaticData.Bytecode
{
    [DisplayName("Primitive/Float")]
    public class Float : Variable<float>
    {
        public override Type Type => Type.Float;
        public override bool IsComputedAtRuntime => false;

        public override float Value
        {
            get => value;
            set => this.value = value;
        }

        [SerializeField]
        private float value;
    }
}