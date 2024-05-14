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

        public virtual void OnBeforeExecute(S source, T target, BattleEngine battleEngine) { }
        public void OnBeforeExecute(BattleEngine battleEngine) => OnBeforeExecute(Source, Target, battleEngine);

        public abstract void Execute(S source, T target, BattleEngine battleEngine);
        public void Execute(BattleEngine battleEngine) => Execute(Source, Target, battleEngine);

        public virtual void OnAfterExecute(S source, T target, BattleEngine battleEngine) { }
        public void OnAfterExecute(BattleEngine battleEngine) => OnAfterExecute(Source, Target, battleEngine);

        public abstract string Log();
    }

    public abstract class BattleEvent<T> : IBattleEvent
    {
        public T Target { get; private set; }

        public BattleEvent(T target)
        {
            Target = target;
        }
        public virtual void OnBeforeExecute(T target, BattleEngine battleEngine) { }
        public void OnBeforeExecute(BattleEngine battleEngine) => OnBeforeExecute(Target, battleEngine);

        public abstract void Execute(T target, BattleEngine battleEngine);
        public void Execute(BattleEngine battleEngine) => Execute(Target, battleEngine);

        public virtual void OnAfterExecute(T target, BattleEngine battleEngine) { }
        public void OnAfterExecute(BattleEngine battleEngine) => OnAfterExecute(Target, battleEngine);

        public abstract string Log();
    }
}