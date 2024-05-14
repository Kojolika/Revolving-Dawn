namespace Fight.Events
{
    public interface IBattleEvent
    {
        void Execute(BattleEngine battleEngine);
        string Log();
    }
}