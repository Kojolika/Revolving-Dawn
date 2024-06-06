using UnityEngine;

namespace Models.Mana
{
    [CreateAssetMenu(fileName = nameof(ManaSODefinition), menuName = "RevolvingDawn/Mana/New " + nameof(ManaSODefinition))]
    public class ManaSODefinition : ScriptableObject
    {
        public string Name => name;
        public Color Color => color;

        [SerializeField] private new string name;

        [SerializeField] private Color color;
    }
}
