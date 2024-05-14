using System.Linq;
using Models.Buffs;

namespace Fight.Events
{
    public abstract class BattleEventTargetingIBuffable<T> : BattleEvent<T>
        where T : IBuffable
    {
        protected BattleEventTargetingIBuffable(T target) : base(target)
        {

        }
        public override void OnBeforeExecute(T target, BattleEngine battleEngine)
        {
            target.Buffs
                .Select(buff => BuffTriggeredEventFactory.GenerateTriggeredEvent(this, buff, BuffTriggeredEventFactory.Timing.Before))
                .Where(triggeredEvent => triggeredEvent != null)
                .ToList()
                .ForEach(triggeredEvent => battleEngine.InsertBeforeEvent(this, triggeredEvent));
        }

        public override void OnAfterExecute(T target, BattleEngine battleEngine)
        {
            target.Buffs
                .Select(buff => BuffTriggeredEventFactory.GenerateTriggeredEvent(this, buff, BuffTriggeredEventFactory.Timing.After))
                .Where(triggeredEvent => triggeredEvent != null)
                .ToList()
                .ForEach(triggeredEvent => battleEngine.InsertAfterEvent(this, triggeredEvent));
        }
    }

    public abstract class BattleEventTargettingIBuffable<S, T> : BattleEvent<S, T>
        where S : IBuffable
        where T : IBuffable
    {
        protected BattleEventTargettingIBuffable(S source, T target) : base(source, target)
        {

        }

        public override void OnBeforeExecute(S source, T target, BattleEngine battleEngine)
        {
            source.Buffs
                .Select(buff => BuffTriggeredEventFactory.GenerateTriggeredEvent(this, buff, BuffTriggeredEventFactory.Timing.Before))
                .Where(triggeredEvent => triggeredEvent != null)
                .ToList()
                .ForEach(triggeredEvent => battleEngine.InsertBeforeEvent(this, triggeredEvent));

            target.Buffs
                .Select(buff => BuffTriggeredEventFactory.GenerateTriggeredEvent(this, buff, BuffTriggeredEventFactory.Timing.Before))
                .Where(triggeredEvent => triggeredEvent != null)
                .ToList()
                .ForEach(triggeredEvent => battleEngine.InsertBeforeEvent(this, triggeredEvent));
        }

        public override void OnAfterExecute(S source, T target, BattleEngine battleEngine)
        {
            source.Buffs
                .Select(buff => BuffTriggeredEventFactory.GenerateTriggeredEvent(this, buff, BuffTriggeredEventFactory.Timing.After))
                .Where(triggeredEvent => triggeredEvent != null)
                .ToList()
                .ForEach(triggeredEvent => battleEngine.InsertAfterEvent(this, triggeredEvent));

            target.Buffs
                .Select(buff => BuffTriggeredEventFactory.GenerateTriggeredEvent(this, buff, BuffTriggeredEventFactory.Timing.After))
                .Where(triggeredEvent => triggeredEvent != null)
                .ToList()
                .ForEach(triggeredEvent => battleEngine.InsertAfterEvent(this, triggeredEvent));
        }
    }
}