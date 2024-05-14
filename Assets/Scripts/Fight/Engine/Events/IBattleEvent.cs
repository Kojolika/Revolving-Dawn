namespace Fight.Events
{
    public interface IBattleEvent
    {
        void OnBeforeExecute(BattleEngine battleEngine);
        void Execute(BattleEngine battleEngine);
        void OnAfterExecute(BattleEngine battleEngine);
        string Log();
    }
}