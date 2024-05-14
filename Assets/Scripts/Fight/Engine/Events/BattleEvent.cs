using System.Linq;
using Models.Buffs;

namespace Fight.Events
{
    public abstract class BattleEvent<S, T> : IBattleEvent
    {
        public S Source { get; private set; }
        public T Target { get; private set; }

        public BattleEvent(S source, T target)
        {
            Source = source;
            Target = target;
        }

        public abstract void Execute(S source, T target, BattleEngine battleEngine);
        public void Execute(BattleEngine battleEngine) => Execute(Source, Target, battleEngine);
        public abstract string Log();
    }

    public abstract class BattleEvent<T> : IBattleEvent
    {
        public T Target { get; private set; }

        public BattleEvent(T target)
        {
            Target = target;
        }

        public abstract void Execute(T target, BattleEngine battleEngine);
        public void Execute(BattleEngine battleEngine) => Execute(Target, battleEngine);
        public abstract string Log();
    }
}