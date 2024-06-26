using Serialization;
using UnityEngine;

namespace Models.Mana
{
    [CreateAssetMenu(fileName = nameof(ManaSODefinition), menuName = "RevolvingDawn/Mana/New " + nameof(ManaSODefinition))]
    public class ManaSODefinition : ScriptableObject, IHaveSerializableRepresentation<ManaDefinition>
    {
        [SerializeField] private Color color;
        public Color Color => color;

        private ManaDefinition representation;
        public ManaDefinition Representation
        {
            get
            {
                representation ??= new ManaDefinition(this);
                return representation;
            }
            private set => representation = value;
        }
    }

    public class ManaDefinition
    {
        public readonly string Name;
        public readonly Color Color;

        public ManaDefinition(ManaSODefinition manaSODefinition)
        {
            Name = manaSODefinition.name;
            Color = manaSODefinition.Color;
        }
    }
}
