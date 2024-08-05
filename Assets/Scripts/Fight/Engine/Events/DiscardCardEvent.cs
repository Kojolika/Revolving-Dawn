using Controllers;
using Views;
using Zenject;

namespace Fight.Events
{
    public class DiscardCardEvent : BattleEvent<CardView>
    {
        private readonly PlayerHandController playerHandController;
        public DiscardCardEvent(
            CardView target,
            PlayerHandController playerHandController,
            bool isCharacterAction = false)
            : base(target, isCharacterAction)
        {
            this.playerHandController = playerHandController;
        }

        public override void Execute(CardView target, BattleEngine battleEngine)
        {
            playerHandController.DiscardCard(target.Model);
        }

        public override string Log() => $"Player discarded {Target.Model.Name}";

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }

        public class Factory : PlaceholderFactory<CardView, DiscardCardEvent> { }
    }
}