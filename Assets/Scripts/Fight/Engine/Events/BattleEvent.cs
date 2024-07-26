using Zenject;

namespace Fight.Events
{
    public abstract class BattleEvent<S, T> : BattleEvent
    {
        public S Source { get; private set; }
        public T Target { get; private set; }
        public BattleEvent(S source, T target, bool isCharacterAction = false) : base(isCharacterAction)
        {
            Source = source;
            Target = target;
        }

        public virtual void OnBeforeExecute(S source, T target, BattleEngine battleEngine) { }
        public override void OnBeforeExecute(BattleEngine battleEngine) => OnBeforeExecute(Source, Target, battleEngine);

        public abstract void Execute(S source, T target, BattleEngine battleEngine);
        public override void Execute(BattleEngine battleEngine) => Execute(Source, Target, battleEngine);

        public virtual void OnAfterExecute(S source, T target, BattleEngine battleEngine) { }
        public override void OnAfterExecute(BattleEngine battleEngine) => OnAfterExecute(Source, Target, battleEngine);

        public class BattleEventFactoryST<E> : PlaceholderFactory<S, T, E> where E : BattleEvent<S,T> { }
    }

    public abstract class BattleEvent<T> : BattleEvent
    {
        public T Target { get; private set; }

        public BattleEvent(T target, bool isCharacterAction = false) : base(isCharacterAction)
        {
            Target = target;
        }
        public virtual void OnBeforeExecute(T target, BattleEngine battleEngine) { }
        public override void OnBeforeExecute(BattleEngine battleEngine) => OnBeforeExecute(Target, battleEngine);

        public abstract void Execute(T target, BattleEngine battleEngine);
        public override void Execute(BattleEngine battleEngine) => Execute(Target, battleEngine);

        public virtual void OnAfterExecute(T target, BattleEngine battleEngine) { }
        public override void OnAfterExecute(BattleEngine battleEngine) => OnAfterExecute(Target, battleEngine);

        public class BattleEventFactoryT<E> : PlaceholderFactory<T, E> where E : BattleEvent<T> { }
    }

    public abstract class BattleEvent : IBattleEvent
    {
        public bool IsCharacterAction { get; protected set; }
        public BattleEvent(bool isCharacterAction = false)
        {
            IsCharacterAction = isCharacterAction;
        }
        public virtual void OnBeforeExecute(BattleEngine battleEngine) { }
        public abstract void Execute(BattleEngine battleEngine);
        public virtual void OnAfterExecute(BattleEngine battleEngine) { }
        public abstract void Undo();
        public abstract string Log();

        public class BattleEventFactory<E> : PlaceholderFactory<E> where E : BattleEvent { }
    }
}