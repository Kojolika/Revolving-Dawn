using Newtonsoft.Json;
using Serialization;
using Tooling.Logging;
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
                MyLogger.Log($"Getting mana rep, : {representation == null}, {representation?.Color}, {representation?.Name}");
                if (representation == null)
                {
                    MyLogger.Log($"Rep is null");
                    representation = new ManaModel(this);
                    MyLogger.LogError($"Rep color is now {representation.Color}");
                }
                return representation;
            }
        }
    }

    [System.Serializable]
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
