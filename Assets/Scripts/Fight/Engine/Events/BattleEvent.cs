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

        public abstract void Execute(S source, T target);
        public void Execute() => Execute(Source, Target);
        public abstract string Log();
    }

    public abstract class BattleEvent<T> : IBattleEvent
    {
        public T Target { get; private set; }

        public BattleEvent(T target)
        {
            Target = target;
        }

        public abstract void Execute(T target);
        public void Execute() => Execute(Target);
        public abstract string Log();
    }
}