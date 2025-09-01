namespace Fight.Events
{
    public interface IBattleEvent
    {
        void   Execute(Context fightContext);
        void   Undo();
        string Log();
    }

    public interface IBattleEvent<out T> : IBattleEvent
    {
        T Target { get; }
    }

    public interface IBattleEvent<out TSource, out TTarget> : IBattleEvent<TTarget>
    {
        TSource Source { get; }
    }

    public abstract class BattleEvent<T> : IBattleEvent<T>
    {
        public T Target { get; }

        public BattleEvent(T target)
        {
            Target = target;
        }

        public abstract void   Execute(Context fightContext);
        public abstract void   Undo();
        public abstract string Log();
    }

    public abstract class BattleEvent<TSource, TTarget> : BattleEvent<TTarget>
    {
        public TSource Source { get; }

        protected BattleEvent(TTarget target, TSource source) : base(target)
        {
            Source = source;
        }
    }
}