using Newtonsoft.Json;
using Serialization;
using UnityEngine;

namespace Models.Mana
{
    [CreateAssetMenu(fileName = nameof(ManaSODefinition), menuName = "RevolvingDawn/Mana/New " + nameof(ManaSODefinition))]
    public class ManaSODefinition : ScriptableObject, IHaveSerializableRepresentation<ManaModel>
    {
        [SerializeField] private Color color;
        public Color Color => color;

        private ManaModel representation;
        public ManaModel Representation
        {
            get
            {
                representation ??= new ManaModel(this);
                return representation;
            }
            private set => representation = value;
        }
    }

    public class ManaModel
    {
        public readonly string Name;
        public readonly Color Color;

        [JsonConstructor]
        public ManaModel() { }

        public ManaModel(ManaSODefinition manaSODefinition)
        {
            Name = manaSODefinition.name;
            Color = manaSODefinition.Color;
        }
    }
}
