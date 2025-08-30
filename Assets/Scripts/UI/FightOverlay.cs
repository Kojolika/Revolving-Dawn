using System.Collections.Generic;
using System.Linq;
using Fight;
using Fight.Engine;
using Fight.Events;
using Models;
using Models.Characters;
using Models.Characters.Player;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace UI
{
    public class FightOverlay : MonoBehaviour, IEventSubscriber<PhaseEndedEvent>
    {
        [SerializeField] Button endTurnButton;
        [SerializeField] Canvas canvas;

        public Canvas Canvas => canvas;

        private RunDefinition  currentRun;
        private PlayerHandView playerHandView;
        private BattleEngine   battleEngine;

        /// <summary>
        /// Which participant this view is overlay is currently for.
        /// TODO: Best way to get this reference?
        /// </summary>
        private ICardDeckParticipant currentParticipant;

        [Zenject.Inject]
        private void Construct(RunDefinition currentRun, PlayerHandView playerHandView, BattleEngine battleEngine)
        {
            this.currentRun     = currentRun;
            this.playerHandView = playerHandView;
            this.battleEngine   = battleEngine;

            endTurnButton.onClick.AddListener(EndPlayerTurn);
            this.battleEngine.SubscribeToEvent<TurnStartedEvent>(this);
        }

        private void EndPlayerTurn()
        {
            endTurnButton.interactable = false;
            var playerCharacter = currentRun.PlayerCharacter;

            foreach (var cardView in playerHandView.CardViewsLookup.Values)
            {
                battleEngine.AddEvent(new DiscardCardEvent(currentParticipant, cardView.Model));
            }

            battleEngine.AddEvent(new TurnEndedEvent(playerCharacter));

            // TODO: Add turn ended internal event for enemies
            /*var enemies = currentRun.CurrentFight.EnemyTeam.Members
                                    .Select(member => member as Enemy);
            var enemyEvents = enemies
                             .SelectMany(enemy => enemy.NextMove.MoveEffects)
                             .SelectMany(effect =>
                              {
/*                     var target = effect.Targeting switch
                    {
                        Cards.Targeting.Options.
                    }; #1#
                                  return effect.Execute(new List<IHealth> { playerCharacter });
                              });

            foreach (var enemyEvent in enemyEvents)
            {
                battleEngine.AddEvent(enemyEvent);
            }*/
        }

        public void OnEvent(PhaseEndedEvent eventData)
        {
            if (eventData.Target.IsPlayerTeam())
            {
                endTurnButton.interactable = true;
            }
        }

        private void OnDestroy()
        {
            endTurnButton?.onClick?.RemoveAllListeners();
            battleEngine?.UnsubscribeFromEvent<TurnEndedEvent>(this);
        }
    }
}