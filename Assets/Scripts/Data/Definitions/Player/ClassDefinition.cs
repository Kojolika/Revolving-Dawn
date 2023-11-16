using UnityEngine;
using Utils.Attributes;

namespace Data.Definitions.Player
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Data/" + nameof(ClassDefinition), fileName = nameof(ClassDefinition))]
    public class ClassDefinition : ScriptableObject
    {
        [ScriptableObjectId]
        public string ID;

        [SerializeField]
        public string Name;

        [SerializeField]
        public string CharacterAsset;
    }
}