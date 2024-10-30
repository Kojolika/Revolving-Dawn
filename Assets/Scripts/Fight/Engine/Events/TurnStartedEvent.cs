using Models.Characters;
using Systems.Managers;
using Zenject;

namespace Fight.Events
{
    public class TurnStartedEvent : BattleEventTargetingIBuffable<Character>
    {
        private readonly DrawCardEvent.BattleEventFactoryT<DrawCardEvent> drawCardFactory;
        private readonly PlayerDataManager playerDataManager;
        public TurnStartedEvent(
            Character target,
            DrawCardEvent.BattleEventFactoryT<DrawCardEvent> drawCardFactory,
            PlayerDataManager playerDataManager
        ) : base(target)
        {
            this.drawCardFactory = drawCardFactory;
            this.playerDataManager = playerDataManager;
        }

        public override void Execute(Character target, BattleEngine battleEngine) { }

        public override void OnAfterExecute(Character target, BattleEngine battleEngine)
        {
            base.OnAfterExecute(target, battleEngine);
            if (target is PlayerCharacter playerCharacter)
            {
                playerCharacter.Decks.Draw = playerCharacter.Decks.Full;
                for (int i = 0; i < playerCharacter.DrawAmount; i++)
                {
                    battleEngine.InsertAfterEvent(drawCardFactory.Create(playerCharacter), this);
                }

                foreach (var enemy in playerDataManager.CurrentPlayerDefinition.CurrentRun.CurrentFight.EnemyTeam.Members)
                {
                    (enemy as Enemy)?.SelectMove();
                }
            }
        }

        public override string Log() => $"{Target.Name}'s turn started!";

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}