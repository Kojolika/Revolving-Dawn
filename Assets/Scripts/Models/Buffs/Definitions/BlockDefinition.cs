using UnityEngine;

namespace Models.Buffs
{
    public class BlockDefinition : BuffDefinition, IStackableBuffDefinition
    {
        [SerializeField] new string name;
        [SerializeField] ulong amountLostPerEvent;
        [SerializeField] ulong maxStackSize;
        [SerializeField] Sprite icon;

        public override string Name => name;
        public ulong AmountLostPerEvent => amountLostPerEvent;
        public ulong MaxStackSize => maxStackSize;
    }
}