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

        public override string Log() => $"Buff {Buff.Definition.Name} triggered off of {nameof(TEvent)}";

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }

    public static class BuffTriggeredEventFactory
    {
        public enum Timing
        {
            Before,
            After
        }
        public static BuffTriggeredEvent<TEvent> GenerateTriggeredEvent<TEvent>(TEvent eventToCheck, Buff buff, Timing timing) where TEvent : IBattleEvent
        {
            if (timing == Timing.Before)
            {
                if (buff.Definition is ITriggerableBuffBefore<TEvent> triggerableBuff)
                {
                    return new BuffTriggeredEvent<TEvent>(eventToCheck, triggerableBuff.OnBeforeTrigger, buff);
                }

                return null;
            }
            else
            {
                if (buff.Definition is ITriggerableBuffAfter<TEvent> triggerableBuff)
                {
                    return new BuffTriggeredEvent<TEvent>(eventToCheck, triggerableBuff.OnAfterTrigger, buff);
                }

                return null;
            }
        }
    }
}