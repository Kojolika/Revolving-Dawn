using Models.Fight;
using Models.Characters;
using Systems.Managers;

namespace Fight.Events
{
    public class PhaseEndedEvent : BattleEvent<Team>
    {
        private readonly PlayerDataManager playerDataManager;

        public PhaseEndedEvent(Team target,
            PlayerDataManager playerDataManager,
            bool isCharacterAction = false) : base(target, isCharacterAction)
        {
            this.playerDataManager = playerDataManager;
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }

        public override string Log() => $"{Target.Name} teams turn ended!";

        public override void Execute(Team target, BattleEngine battleEngine)
        {
            foreach (var member in target.Members)
            {
                battleEngine.AddEvent(new TurnEndedEvent(member));
            }
        }
    }
}