using Controllers;
using Cysharp.Threading.Tasks;
using Models.Characters;
using Tooling.Logging;

namespace Fight.Events
{
    public class DrawCardEvent : BattleEventTargetingIBuffable<PlayerCharacter>
    {
        private PlayerHandController playerHandController;

        public DrawCardEvent(PlayerCharacter target, PlayerHandController playerHandController, bool isCharacterAction = false) : base(target, isCharacterAction)
        {
            this.playerHandController = playerHandController;
        }

        public override void Execute(PlayerCharacter target, BattleEngine battleEngine)
        {
            playerHandController.DrawCard();
        }

        public override string Log() => $"{Target} drew a card!";

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}