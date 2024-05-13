using Fight.Events;
using UnityEngine;
using Utils.Attributes;

namespace Models.Buffs
{
    public class BuffDefinition : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Sprite icon;
        [SerializeReference, DisplayInterface(typeof(IBattleEvent))] private IBattleEvent stackLossEvent;
        [SerializeField] private AmountLost<ulong> amountLostPerEvent;
        [SerializeField] private ulong maxStackSize;
        

        public string Name => name;
        public Sprite Icon => icon;
        public IBattleEvent StackLossEvent => stackLossEvent;
        public AmountLost<ulong> AmountLostPerEvent => amountLostPerEvent;
        public ulong MaxStackSize => maxStackSize;
    }
}