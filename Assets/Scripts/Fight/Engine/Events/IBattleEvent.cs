namespace Fight.Events
{
    public interface IBattleEvent
    {
        void Execute();
        string Log();
    }
}