using System;
using UnityEngine;

namespace Models.Buffs
{
    [Serializable]
    public class Buff : IRuntimeModel<BuffDefinition>
    {
        [SerializeField] private BuffDefinition buffDefinition;
        [SerializeField] private ulong stacksize;

        public ulong StackSize
        {
            get => stacksize;
            set => stacksize = value;
        }
        public BuffDefinition Definition => buffDefinition;
    }
}