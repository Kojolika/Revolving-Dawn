using UnityEngine;
using Newtonsoft.Json;

namespace Models.Mana
{
    [CreateAssetMenu(fileName = nameof(ManaDefinition), menuName = "RevolvingDawn/Mana/New " + nameof(ManaDefinition))]
    public class ManaDefinition : ScriptableObject
    {
        public string Name => name;
        public Color Color => color;

        [SerializeField] private new string name;

        [SerializeField] private Color color;
    }
}
