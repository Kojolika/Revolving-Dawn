namespace Fight.Events
{
    public interface IBattleEvent
    {
        bool IsCharacterAction { get; }
        void OnBeforeExecute(BattleEngine battleEngine);
        void Execute(BattleEngine battleEngine);
        void OnAfterExecute(BattleEngine battleEngine);
        void Undo();
        string Log();
    }
}