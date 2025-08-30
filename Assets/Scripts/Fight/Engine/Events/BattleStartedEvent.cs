namespace Fight.Events
{
    public class BattleStartedEvent : IBattleEvent
    {
        public string Log() => "Fight started!";

        public void Execute(Context fightContext)
        {
        }

        public void Undo()
        {
        }
    }
}