using Systems.Managers;

namespace Fight.Events
{
    public class BattleStartedEvent : BattleEvent<FightManager_Obsolete>
    {
        public BattleStartedEvent(FightManager_Obsolete target) : base(target) { }
        public override void Execute(FightManager_Obsolete target, BattleEngine battleEngine) { }
        public override string Log() => "Fight started!";

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}