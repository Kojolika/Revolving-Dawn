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

        private RunDefinition currentRun;
        private PlayerHandView playerHandView;
        private DiscardCardEvent.Factory discardCardEventFactory;
        private BattleEngine battleEngine;

        [Zenject.Inject]
        private void Construct(RunDefinition currentRun,
            PlayerHandView playerHandView,
            DiscardCardEvent.Factory discardCardEventFactory,
            BattleEngine battleEngine)
        {
            this.currentRun = currentRun;
            this.playerHandView = playerHandView;
            this.discardCardEventFactory = discardCardEventFactory;
            this.battleEngine = battleEngine;

            endTurnButton.onClick.AddListener(EndPlayerTurn);
            this.battleEngine.SubscribeToEvent<TurnStartedEvent>(this);
        }
        
        private void EndPlayerTurn()
        {
            endTurnButton.interactable = false;
            var playerCharacter = currentRun.PlayerCharacter;


            foreach (var cardView in playerHandView.CardViewsLookup.Values)
            {
                battleEngine.AddEvent(discardCardEventFactory.Create(cardView));
            }

            battleEngine.AddEvent(new TurnEndedEvent(playerCharacter));

            var enemies = currentRun.CurrentFight.EnemyTeam.Members
                .Select(member => member as Enemy);
            var enemyEvents = enemies
                .SelectMany(enemy => enemy.NextMove.MoveEffects)
                .SelectMany(effect =>
                {
/*                     var target = effect.Targeting switch
                    {
                        Cards.Targeting.Options.
                    }; */
                    return effect.Execute(new List<IHealth> { playerCharacter });
                });

            foreach (var enemyEvent in enemyEvents)
            {
                battleEngine.AddEvent(enemyEvent);
            }
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
