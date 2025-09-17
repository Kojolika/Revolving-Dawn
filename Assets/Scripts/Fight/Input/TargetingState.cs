using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Fight.Engine;
using Fight.Events;
using Fight.Input;
using Models.Cards;
using Tooling.StaticData.Data;
using Systems.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;
using Zenject;

namespace Fight
{
    public class TargetingState : PlayerInputState
    {
        private readonly CardView               cardView;
        private readonly DefaultState           defaultState;
        private readonly TargetingArrowView     targetingArrowView;
        private readonly PlayerHandViewSettings playerHandViewSettings;
        private readonly LevelView              levelView;
        private readonly Camera                 mainCamera;
        private readonly Context                fightContext;

        private bool             shouldDrawCurve;
        private IParticipantView previousFrameHoveredParticipant;

        private Material enemyOutlineMaterial;
        private Material friendlyOutlineMaterial;
        private Material defaultSpriteMaterial;

        /// <summary>
        /// A card may have multiple targeting options (enemy, enemy, all enemies) this caches how many options it has
        /// </summary>
        private int numberOfTargetingOptions;

        /// <summary>
        /// We'll play the card once we satisfy all targeting options. This tracks how many we've acquired so far
        /// </summary>
        private bool[] acquiredTarget;

        public TargetingState(
            InputActionAsset       playerHandInputActionAsset,
            PlayerHandView         playerHandView,
            CardView               cardView,
            DefaultState           defaultState,
            TargetingArrowView     targetingArrowView,
            PlayerHandViewSettings playerHandViewSettings,
            LevelView              levelView,
            AddressablesManager    addressablesManager,
            Camera                 mainCamera,
            Context                fightContext)
            : base(playerHandInputActionAsset, playerHandView)
        {
            this.cardView               = cardView;
            this.defaultState           = defaultState;
            this.targetingArrowView     = targetingArrowView;
            this.playerHandViewSettings = playerHandViewSettings;
            this.levelView              = levelView;
            this.mainCamera             = mainCamera;
            this.fightContext           = fightContext;

            var cancellationToken = playerHandView.GetCancellationTokenOnDestroy();
            _ = addressablesManager.LoadGenericAsset(
                playerHandViewSettings.EnemyOutlineMaterial,
                () => cancellationToken.IsCancellationRequested,
                asset => enemyOutlineMaterial = asset
            );
            _ = addressablesManager.LoadGenericAsset(
                playerHandViewSettings.FriendlyOutlineMaterial,
                () => cancellationToken.IsCancellationRequested,
                asset => friendlyOutlineMaterial = asset
            );
            _ = addressablesManager.LoadGenericAsset(
                playerHandViewSettings.DefaultSpriteMaterial,
                () => cancellationToken.IsCancellationRequested,
                asset => defaultSpriteMaterial = asset
            );
        }

        public override void OnEnter()
        {
            base.OnEnter();

            numberOfTargetingOptions = cardView.Model.Model.TargetingOptions.Count;
            acquiredTarget           = new bool[numberOfTargetingOptions];

            for (int i = 0; i < numberOfTargetingOptions; i++)
            {
                var targetingOption = cardView.Model.Model.TargetingOptions[i];
                acquiredTarget[i] = targetingOption is not Targeting.Options.Enemy and not Targeting.Options.Friendly;
            }


            shouldDrawCurve = acquiredTarget.Any(acquired => !acquired);
            targetingArrowView.SetActive(shouldDrawCurve);

            if (cardView.Model.Model.TargetingOptions.Any(option => option == Targeting.Options.Enemy
                                                                      || option == Targeting.Options.RandomEnemy
                                                                      || option == Targeting.Options.AllEnemies
                                                                      || option == Targeting.Options.All))
            {
                foreach (var enemy in levelView.EnemyLookup.Values)
                {
                    enemy.HighlightEnemy();
                }
            }

            if (cardView.Model.Model.TargetingOptions.Any(option => option == Targeting.Options.Friendly
                                                                      || option == Targeting.Options.All))
            {
                foreach (var player in levelView.PlayerLookup.Values)
                {
                    player.HighlightFriendly();
                }
            }
        }

        public override void OnExit()
        {
            targetingArrowView.SetActive(false);

            foreach (var enemy in levelView.EnemyLookup.Values)
            {
                enemy.Unhighlight();
            }

            levelView.PlayerLookup.Values.First().Unhighlight();
        }

        public override void Update()
        {
            if (!dragAction.inProgress)
            {
                NextState = defaultState;

                // If there is a play affect that needs a specific target and there is no target chosen,
                // return to the default state (prevent the card from being played)
                if (shouldDrawCurve && previousFrameHoveredParticipant == null && previousFrameHoveredParticipant is not EnemyView)
                {
                    return;
                }

                fightContext.BattleEngine.AddEvent(new PlayCardEvent(levelView.PlayerLookup.Values.First().Model as ICardDeckParticipant, cardView.Model));

                return;
            }

            var playerInputScreenPosition = dragAction.ReadValue<Vector2>();
            previousFrameHoveredParticipant = PollCharacterHovering();
            if (playerHandView.Camera.ScreenToViewportPoint(playerInputScreenPosition).y < playerHandViewSettings.PositionOnScreenWhereTargetingStarts)
            {
                NextState = defaultState;
                return;
            }

            if (shouldDrawCurve)
            {
                targetingArrowView.DrawCurveForCardAndScreenPoint(cardView.transform.position, playerInputScreenPosition);
            }
            else
            {
                var playerInputWorldPosition = playerHandView.Camera.ScreenToWorldPoint(new Vector3(playerInputScreenPosition.x, playerInputScreenPosition.y));
                cardView.transform.position = new Vector3(playerInputWorldPosition.x,
                                                          playerInputWorldPosition.y,
                                                          cardView.transform.position.z
                );
            }
        }

        private IParticipantView PollCharacterHovering()
        {
            var ray     = mainCamera.ScreenPointToRay(dragAction.ReadValue<Vector2>());
            var numHits = Physics.RaycastNonAlloc(ray, raycastHitsBuffer, 500.0F);

            for (int i = 0; i < numHits; i++)
            {
                var hit           = raycastHitsBuffer[i];
                var characterView = hit.transform.GetComponentInParent<IParticipantView>();
                if (characterView != null)
                {
                    return characterView;
                }
            }

            return null;
        }

        /*private List<Models.IHealth>[] GetTargetsForPlayEffects(LevelView levelView, IParticipantView hoveredParticipantView)
        {
            var numCombatEffects = playEffects.Count;
            var targets          = new List<Models.IHealth>[numCombatEffects];

            for (int i = 0; i < numCombatEffects; i++)
            {
                var targetingList = new List<Models.IHealth>();
                switch (playEffects[i].Targeting)
                {
                    case Targeting.Options.Friendly:
                        targetingList.Add(levelView.PlayerLookup.First().Value.CharacterModel);
                        break;
                    case Targeting.Options.Enemy:
                        targetingList.Add(hoveredParticipantView.Model);
                        break;
                    case Targeting.Options.RandomEnemy:
                        var enemyViewArray = levelView.EnemyLookup.Values.ToArray();
                        var numEnemies     = enemyViewArray.Length;
                        var rng            = new System.Random();
                        var enemyNum       = rng.Next(numEnemies - 1);
                        targetingList.Add(enemyViewArray[enemyNum].EnemyModel);
                        break;
                    case Targeting.Options.AllEnemies:
                        targetingList.AddRange(levelView.EnemyLookup.Values.Select(enemyView => enemyView.EnemyModel));
                        break;
                    case Targeting.Options.All:
                        targetingList.Add(levelView.PlayerLookup.First().Value.CharacterModel);
                        targetingList.AddRange(levelView.EnemyLookup.Values.Select(enemyView => enemyView.EnemyModel));
                        break;
                    case Targeting.Options.None:
                        break;
                }

                targets[i] = targetingList;
            }

            return targets;
        }*/

        public class Factory : PlaceholderFactory<CardView, TargetingState>
        {
        }
    }
}