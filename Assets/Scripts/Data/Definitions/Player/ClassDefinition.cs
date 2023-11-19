using UnityEngine;
using Utils;
using Utils.Attributes;

namespace Data.Definitions.Player
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Data/" + nameof(ClassDefinition), fileName = nameof(ClassDefinition))]
    public class ClassDefinition : ScriptableObject
    {
        [PrimaryKey]
        public ReadOnly<string> ID;

        [SerializeField]
        public ReadOnly<string> Name;

        [SerializeField]
        public ReadOnly<string> CharacterAsset;
    }
}