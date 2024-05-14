using Systems.Managers;

namespace Fight.Events
{
    public class BattleStartedEvent : BattleEvent<FightManager>
    {
        public BattleStartedEvent(FightManager target) : base(target) { }
        public override void Execute(FightManager target, BattleEngine battleEngine) { }
        public override string Log() => "Fight started!";
    }
}