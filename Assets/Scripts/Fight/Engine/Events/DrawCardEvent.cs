using Controllers;
using Models;
using Models.Characters;

namespace Fight.Events
{
    public class DrawCardEvent : BattleEventTargetingIBuffable<PlayerCharacter>
    {
        private readonly PlayerHandController playerHandController;
        public CardModel CardDrawn { get; private set; }

        public DrawCardEvent(PlayerCharacter target, PlayerHandController playerHandController, bool isCharacterAction = false) : base(target, isCharacterAction)
        {
            this.playerHandController = playerHandController;
        }

        public override void Execute(PlayerCharacter target, BattleEngine battleEngine)
        {
            CardDrawn = playerHandController.DrawCard();
        }

        public override string Log() => $"{Target.Name} drew a card!";

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}