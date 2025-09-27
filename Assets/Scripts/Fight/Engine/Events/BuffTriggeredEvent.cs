using System;
using Fight.Engine;
using Models.Buffs;
using Tooling.Logging;
using Tooling.StaticData.Data;
using UnityEngine.Assertions;

namespace Fight.Events
{
    /// <summary>
    /// The intended usage of this to only provide one of <see cref="IBeforeEvent"/> or <see cref="IAfterEvent"/>.
    /// We'll reuse this event for buff triggers.
    /// </summary>
    public class BuffTriggeredEvent : BattleEvent<ICombatParticipant>
    {
        public readonly IBattleEvent TriggeredBy;
        public readonly IBeforeEvent BeforeEvent;
        public readonly IAfterEvent  AfterEvent;
        public readonly Buff         Buff;
        public readonly int          CurrentStackSize;

        private ICombatParticipant targetParticipant;

        public BuffTriggeredEvent(
            ICombatParticipant target,
            IBattleEvent       triggeredBy,
            IBeforeEvent       onBeforeEvent,
            IAfterEvent        onAfterEvent,
            Buff               buff,
            int                currentStackSize)
            : base(target)
        {
            Assert.IsTrue(onBeforeEvent != null || onAfterEvent != null, "Only one of OnBeforeEvent or OnAfterEvent can be given.");
            Assert.IsFalse(onBeforeEvent != null && onAfterEvent != null, "Only one of OnBeforeEvent or OnAfterEvent can be given.");

            TriggeredBy      = triggeredBy;
            BeforeEvent      = onBeforeEvent;
            AfterEvent       = onAfterEvent;
            Buff             = buff;
            CurrentStackSize = currentStackSize;
        }

        public override void Execute(Context fightContext)
        {
            targetParticipant = Target;
            if (BeforeEvent != null)
            {
                int newStackSize = BeforeEvent.OnBeforeExecute(targetParticipant, fightContext, TriggeredBy, Buff, CurrentStackSize);
                targetParticipant.SetBuff(Buff, newStackSize);
            }

            if (AfterEvent != null)
            {
                int newStackSize = AfterEvent.OnAfterExecute(targetParticipant, fightContext, TriggeredBy, Buff, CurrentStackSize);
                targetParticipant.SetBuff(Buff, newStackSize);
            }
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }

        public override string Log()
        {
            return $"Buff {Buff?.Name} triggered by {targetParticipant?.Name}";
        }
    }
}