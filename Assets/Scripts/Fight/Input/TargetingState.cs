
using System.Linq;
using Cysharp.Threading.Tasks;
using Fight.Input;
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

        private bool shouldDrawCurve = false;

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
            Camera mainCamera)
            : base(playerHandInputActionAsset, playerHandView)
        {
            this.cardView = cardView;
            this.defaultState = defaultState;
            this.targetingArrowView = targetingArrowView;
            this.playerHandViewSettings = playerHandViewSettings;
            this.levelView = levelView;
            this.mainCamera = mainCamera;

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
                return;
            }

            var playerInputScreenPosition = dragAction.ReadValue<Vector2>();
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

        public class Factory : PlaceholderFactory<CardView, TargetingState> { }
    }
}