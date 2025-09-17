using System;
using Fight.Engine;
using Models.Buffs;
using Tooling.StaticData.Data;
using UnityEngine.Assertions;

namespace Fight.Events
{
    /// <summary>
    /// The intended usage of this to only provide one of <see cref="IBeforeEvent"/> or <see cref="IAfterEvent"/>.
    /// We'll reuse this event for buff triggers.
    /// </summary>
    public class BuffTriggeredEvent : BattleEvent<IBattleEvent<ICombatParticipant>>
    {
        public readonly IBeforeEvent BeforeEvent;
        public readonly IAfterEvent  AfterEvent;
        public readonly Buff         Buff;
        public readonly int          CurrentStackSize;

        public BuffTriggeredEvent(
            IBattleEvent<ICombatParticipant> target,
            IBeforeEvent                     onBeforeEvent,
            IAfterEvent                      onAfterEvent,
            Buff                             buff,
            int                              currentStackSize)
            : base(target)
        {
            Assert.IsTrue(onBeforeEvent != null || onAfterEvent != null, "Only one of OnBeforeEvent or OnAfterEvent can be given.");
            Assert.IsFalse(onBeforeEvent != null && onAfterEvent != null, "Only one of OnBeforeEvent or OnAfterEvent can be given.");

            BeforeEvent      = onBeforeEvent;
            AfterEvent       = onAfterEvent;
            Buff             = buff;
            CurrentStackSize = currentStackSize;
        }

        public override void Execute(Context fightContext)
        {
            ICombatParticipant targetParticipant = Target.Target;
            if (BeforeEvent != null)
            {
                targetParticipant.SetBuff(Buff, BeforeEvent.OnBeforeExecute(fightContext, Target, Buff, CurrentStackSize));
            }

            if (AfterEvent != null)
            {
                targetParticipant.SetBuff(Buff, AfterEvent.OnAfterExecute(fightContext, Target, Buff, CurrentStackSize));
            }
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }

        public override string Log()
        {
            throw new NotImplementedException();
        }
    }
}