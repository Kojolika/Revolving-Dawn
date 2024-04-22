using UnityEngine;

namespace Models.Buffs
{
    public class BlockDefinition : ScriptableObject, IStackableBuffDefinition
    {
        [SerializeField] new string name;
        [SerializeField] ulong amountLostPerEvent;
        [SerializeField] ulong maxStackSize;
        [SerializeField] Sprite icon;

        public string Name => name;
        public ulong AmountLostPerEvent => amountLostPerEvent;
        public ulong MaxStackSize => maxStackSize;
    }
}