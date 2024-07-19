using Controllers;
using Models;
using System.Collections.Generic;
using System.Linq;
using Tooling.Logging;
using Views;

namespace Fight.Events
{
    public class PlayCardEvent : BattleEvent<CardView, List<IHealth>[]>
    {
        private readonly PlayerHandController playerHandController;
        public CardModel CardModelPlayed { get; private set; }
        public CardView CardViewPlayed { get; private set; }
        public PlayCardEvent(CardView target, List<IHealth>[] cardTargets, PlayerHandController playerHandController) : base(target, cardTargets)
        {
            this.playerHandController = playerHandController;
            IsCharacterAction = true;
        }

        public override void Execute(CardView target, List<IHealth>[] cardTargets, BattleEngine battleEngine)
        {
            CardViewPlayed = target;
            CardModelPlayed = target.Model;
            playerHandController.PlayCard(target.Model);
        }

        public override void OnAfterExecute(BattleEngine battleEngine)
        {
            var cardPlayAffects = Source.Model.PlayEffects;
            var targetsLength = Target.Length;
            if (targetsLength != cardPlayAffects.Count)
            {
                MyLogger.LogError($"The targets given to this event {this} must match the size of the cards play affects.");
            }

            var eventsFromCardPlayed = new List<IBattleEvent>();
            for (int i = 0; i < targetsLength; i++)
            {
                eventsFromCardPlayed.AddRange(cardPlayAffects[i].Execute(Target[i]));
            }

            foreach (var battleEvent in eventsFromCardPlayed)
            {
                battleEngine.InsertAfterEvent(battleEvent, this);
            }
        }

        public override string Log() => $"Played card {Source.Model.Name}";

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}