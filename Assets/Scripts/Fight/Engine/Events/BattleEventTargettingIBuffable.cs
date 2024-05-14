using System.Linq;
using Models.Buffs;

namespace Fight.Events
{
    public abstract class BattleEventTargettingIBuffable<T> : BattleEvent<T>
        where T : IBuffable
    {
        protected BattleEventTargettingIBuffable(T target) : base(target)
        {

        }

        public override void Execute(T target, BattleEngine battleEngine)
        {
            target.Buffs
                .Select(buff => buff.EventOccured(this))
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

        public override void Execute(S source, T target, BattleEngine battleEngine)
        {
            target.Buffs
                .Select(buff => buff.EventOccured(this))
                .Where(triggeredEvent => triggeredEvent != null)
                .ToList()
                .ForEach(triggeredEvent => battleEngine.InsertAfterEvent(this, triggeredEvent));
        }
    }
}