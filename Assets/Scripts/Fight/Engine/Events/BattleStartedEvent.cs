namespace Fight.Events
{
    public class BattleStartedEvent : BattleEvent
    {
        public override void Execute(BattleEngine battleEngine) { }
        public override string Log() => "Fight started!";
        public override void Undo() { }
    }
}