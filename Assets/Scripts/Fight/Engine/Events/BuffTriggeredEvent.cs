using System;
using Models.Buffs;

namespace Fight.Events
{
    public class BuffTriggeredEvent<TEvent> : BattleEvent<TEvent>
        where TEvent : IBattleEvent
    {
        public delegate ulong TriggerEffectDelegate(TEvent triggerEvent, Buff buff);


        public Buff Buff { get; private set; }
        public TriggerEffectDelegate TriggerEffect { get; private set; }

        public BuffTriggeredEvent(TEvent target, TriggerEffectDelegate triggerEffect, Buff buff) : base(target)
        {
            Buff = buff;
            TriggerEffect = triggerEffect;
        }

        public override void Execute(TEvent target, BattleEngine battleEngine)
        {
            Buff.StackSize = TriggerEffect.Invoke(target, Buff);
        }

        public override string Log() => $"Buff {Buff.Definition.name} triggered off of {nameof(TEvent)}";
    }
}