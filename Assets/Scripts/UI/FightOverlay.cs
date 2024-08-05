using System;
using System.Collections.Generic;
using System.Linq;
using Fight;
using Fight.Events;
using Models;
using Models.Characters;
using Systems.Managers;
using UI.Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Views;

namespace UI
{
    public class FightOverlay : MonoBehaviour
    {
        [SerializeField] ActionButton endTurnButton;
        [SerializeField] Canvas canvas;

        public Canvas Canvas => canvas;

        private PlayerDataManager playerDataManager;
        private PlayerHandView playerHandView;
        private DiscardCardEvent.Factory discardCardEventFactory;
        private BattleEngine battleEngine;

        [Zenject.Inject]
        private void Construct(PlayerDataManager playerDataManager,
            PlayerHandView playerHandView,
            DiscardCardEvent.Factory discardCardEventFactory,
            BattleEngine battleEngine)
        {
            this.playerDataManager = playerDataManager;
            this.playerHandView = playerHandView;
            this.discardCardEventFactory = discardCardEventFactory;
            this.battleEngine = battleEngine;

            endTurnButton.OnPerformed += EndPlayerTurn;
            battleEngine.EventOccurred += OnPlayerTurnStarted;
        }

        private void OnPlayerTurnStarted(IBattleEvent battleEvent)
        {
            if (battleEvent is TurnStartedEvent turnStartedEvent && turnStartedEvent.Target is PlayerCharacter)
            {
                endTurnButton.interactable = true;
            }
        }

        private void EndPlayerTurn(InputAction.CallbackContext context)
        {
            endTurnButton.interactable = false;
            var playerCharacter = playerDataManager.CurrentPlayerDefinition.CurrentRun.PlayerCharacter;


            foreach (var cardView in playerHandView.CardViewsLookup.Values)
            {
                battleEngine.AddEvent(discardCardEventFactory.Create(cardView));
            }

            battleEngine.AddEvent(new TurnEndedEvent(playerCharacter));

            var enemies = playerDataManager.CurrentPlayerDefinition.CurrentRun.CurrentFight.Enemies;
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
    }
}
