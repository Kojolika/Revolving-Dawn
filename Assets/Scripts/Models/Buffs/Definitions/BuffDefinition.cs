using Fight.Events;
using UnityEngine;
using Utils.Attributes;

namespace Models.Buffs
{
    public class BuffDefinition : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Sprite icon;
        [SerializeField] private ulong maxStackSize;
        

        public string Name => name;
        public Sprite Icon => icon;
        public ulong MaxStackSize => maxStackSize;
    }
}