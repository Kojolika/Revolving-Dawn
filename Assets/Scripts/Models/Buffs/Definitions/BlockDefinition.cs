using UnityEngine;

namespace Models.Buffs
{
    [CreateAssetMenu(fileName = "New " + nameof(BlockDefinition), menuName = "RevolvingDawn/Buffs/" + nameof(BlockDefinition))]
    public class BlockDefinition : BuffDefinition, IStackableBuffDefinition
    {
        [SerializeField] new string name;
        [SerializeField] AmountLost<ulong> amountLostPerEvent;
        [SerializeField] ulong maxStackSize;
        [SerializeField] Sprite icon;

        public override string Name => name;
        public ulong MaxStackSize => maxStackSize;
        public AmountLost<ulong> AmountLostPerEvent => amountLostPerEvent;
    }
}