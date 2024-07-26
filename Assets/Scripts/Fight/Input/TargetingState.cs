
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Fight.Events;
using Fight.Input;
using Models.CardEffects;
using Models.Characters;
using Settings;
using Systems.Managers;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;
using Zenject;

namespace Fight
{
    public class TargetingState : PlayerInputState
    {
        private readonly CardView cardView;
        private readonly DefaultState defaultState;
        private readonly TargetingArrowView targetingArrowView;
        private readonly PlayerHandViewSettings playerHandViewSettings;
        private readonly LevelView levelView;
        private readonly Camera mainCamera;
        private readonly BattleEngine battleEngine;
        private readonly PlayCardEvent.BattleEventFactoryST<PlayCardEvent> playCardEventFactory;

        private bool shouldDrawCurve = false;
        private ICharacterView previousFrameHoveredCharacter;

        private Material enemyOutlineMaterial;
        private Material friendlyOutlineMaterial;
        private Material defaultSpriteMaterial;

        public TargetingState(InputActionAsset playerHandInputActionAsset,
            PlayerHandView playerHandView,
            CardView cardView,
            DefaultState defaultState,
            TargetingArrowView targetingArrowView,
            PlayerHandViewSettings playerHandViewSettings,
            LevelView levelView,
            AddressablesManager addressablesManager,
            Camera mainCamera,
            BattleEngine battleEngine,
            PlayCardEvent.BattleEventFactoryST<PlayCardEvent> playCardEventFactory)
            : base(playerHandInputActionAsset, playerHandView)
        {
            this.cardView = cardView;
            this.defaultState = defaultState;
            this.targetingArrowView = targetingArrowView;
            this.playerHandViewSettings = playerHandViewSettings;
            this.levelView = levelView;
            this.mainCamera = mainCamera;
            this.battleEngine = battleEngine;
            this.playCardEventFactory = playCardEventFactory;

            var cancellationToken = playerHandView.GetCancellationTokenOnDestroy();
            _ = addressablesManager.LoadGenericAsset(playerHandViewSettings.EnemyOutlineMaterial,
                () => cancellationToken.IsCancellationRequested,
                asset => enemyOutlineMaterial = asset
            );
            _ = addressablesManager.LoadGenericAsset(playerHandViewSettings.FriendlyOutlineMaterial,
                () => cancellationToken.IsCancellationRequested,
                asset => friendlyOutlineMaterial = asset
            );
            _ = addressablesManager.LoadGenericAsset(playerHandViewSettings.DefaultSpriteMaterial,
                () => cancellationToken.IsCancellationRequested,
                asset => defaultSpriteMaterial = asset
            );
        }

        public override void OnEnter()
        {
            base.OnEnter();

            shouldDrawCurve = cardView.Model.PlayEffects.Any(effect => effect.Targeting == Cards.Targeting.Options.Enemy);
            targetingArrowView.SetActive(shouldDrawCurve);

            if (cardView.Model.PlayEffects.Any(effect => effect.Targeting == Cards.Targeting.Options.Enemy
                || effect.Targeting == Cards.Targeting.Options.RandomEnemy
                || effect.Targeting == Cards.Targeting.Options.AllEnemies
                || effect.Targeting == Cards.Targeting.Options.All))
            {
                foreach (var enemy in levelView.EnemyLookup.Values)
                {
                    enemy.SetMaterial(enemyOutlineMaterial);
                }
            }

            if (cardView.Model.PlayEffects.Any(effect => effect.Targeting == Cards.Targeting.Options.Friendly
                || effect.Targeting == Cards.Targeting.Options.All))
            {
                foreach (var player in levelView.PlayerLookup.Values)
                {
                    player.SetMaterial(friendlyOutlineMaterial);
                }
            }
        }

        public override void OnExit()
        {
            targetingArrowView.SetActive(false);

            foreach (var enemy in levelView.EnemyLookup.Values)
            {
                enemy.SetMaterial(defaultSpriteMaterial);
            }
            levelView.PlayerLookup.Values.First().SetMaterial(defaultSpriteMaterial);
        }

        public override void Update()
        {
            if (!dragAction.inProgress)
            {
                NextState = defaultState;

                // If there is a play affect that needs a specific target and there is no target chosen,
                // return to the default state (prevent the card from being played)
                if (shouldDrawCurve && previousFrameHoveredCharacter == null && previousFrameHoveredCharacter is not EnemyView)
                {
                    return;
                }

                battleEngine.AddEvent(
                    playCardEventFactory.Create(
                        cardView,
                        GetTargetsForPlayEffects(
                            cardView.Model.PlayEffects,
                            levelView,
                            previousFrameHoveredCharacter
                        )
                    )
                );

                return;
            }

            var playerInputScreenPosition = dragAction.ReadValue<Vector2>();
            previousFrameHoveredCharacter = PollCharacterHovering();
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

        private ICharacterView PollCharacterHovering()
        {
            Ray ray = mainCamera.ScreenPointToRay(dragAction.ReadValue<Vector2>());
            var numHits = Physics.RaycastNonAlloc(ray, raycastHitsBuffer, 500.0F);

            for (int i = 0; i < numHits; i++)
            {
                var hit = raycastHitsBuffer[i];
                var characterView = hit.transform.GetComponentInParent<ICharacterView>();
                if (characterView != null)
                {
                    return characterView;
                }
            }
            return null;
        }

        private List<Models.IHealth>[] GetTargetsForPlayEffects(List<ICombatEffect> playEffects,
            LevelView levelView,
            ICharacterView hoveredCharacterView)
        {
            var numCombatEffects = playEffects.Count;
            var targets = new List<Models.IHealth>[numCombatEffects];

            for (int i = 0; i < numCombatEffects; i++)
            {
                var targetingList = new List<Models.IHealth>();
                switch (playEffects[i].Targeting)
                {
                    case Cards.Targeting.Options.Friendly:
                        targetingList.Add(levelView.PlayerLookup.First().Value.CharacterModel);
                        break;
                    case Cards.Targeting.Options.Enemy:
                        targetingList.Add(hoveredCharacterView.CharacterModel);
                        break;
                    case Cards.Targeting.Options.RandomEnemy:
                        var enemyViewArray = levelView.EnemyLookup.Values.ToArray();
                        var numEnemies = enemyViewArray.Length;
                        var rng = new System.Random();
                        var enemyNum = rng.Next(numEnemies - 1);
                        targetingList.Add(enemyViewArray[enemyNum].Enemy);
                        break;
                    case Cards.Targeting.Options.AllEnemies:
                        targetingList.AddRange(levelView.EnemyLookup.Values.Select(enemyView => enemyView.Enemy));
                        break;
                    case Cards.Targeting.Options.All:
                        targetingList.Add(levelView.PlayerLookup.First().Value.CharacterModel);
                        targetingList.AddRange(levelView.EnemyLookup.Values.Select(enemyView => enemyView.Enemy));
                        break;
                    case Cards.Targeting.Options.None:
                        break;
                }
                targets[i] = targetingList;
            }
            return targets;
        }

        public class Factory : PlaceholderFactory<CardView, TargetingState> { }
    }
}