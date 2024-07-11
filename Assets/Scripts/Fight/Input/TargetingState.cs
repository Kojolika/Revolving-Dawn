
using System.Linq;
using Cysharp.Threading.Tasks;
using Fight.Input;
using Settings;
using Systems.Managers;
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

        private bool shouldDrawCurve = false;

        private Material enemyOutlineMaterial;
        private Material friendlyOutlineMaterial;
        private Material spriteMaterial;

        public TargetingState(InputActionAsset playerHandInputActionAsset,
            PlayerHandView playerHandView,
            CardView cardView,
            DefaultState defaultState,
            TargetingArrowView targetingArrowView,
            PlayerHandViewSettings playerHandViewSettings,
            LevelView levelView,
            AddressablesManager addressablesManager)
            : base(playerHandInputActionAsset, playerHandView)
        {
            this.cardView = cardView;
            this.defaultState = defaultState;
            this.targetingArrowView = targetingArrowView;
            this.playerHandViewSettings = playerHandViewSettings;
            this.levelView = levelView;

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
                asset => spriteMaterial = asset
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
                levelView.PlayerLookup.Values.First().SetMaterial(friendlyOutlineMaterial);
            }
        }

        public override void OnExit()
        {
            targetingArrowView.SetActive(false);

            foreach (var enemy in levelView.EnemyLookup.Values)
            {
                enemy.SetMaterial(spriteMaterial);
            }
            levelView.PlayerLookup.Values.First().SetMaterial(spriteMaterial);
        }

        public override void Update()
        {
            if (!dragAction.inProgress)
            {
                NextState = defaultState;
                return;
            }

            var playerInputScreenPosition = dragAction.ReadValue<UnityEngine.Vector2>();

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
                var playerInputWorldPosition = playerHandView.Camera.ScreenToWorldPoint(new UnityEngine.Vector3(playerInputScreenPosition.x, playerInputScreenPosition.y));
                cardView.transform.position = new UnityEngine.Vector3(playerInputWorldPosition.x,
                    playerInputWorldPosition.y,
                    cardView.transform.position.z
                );
            }
        }

        public class Factory : PlaceholderFactory<CardView, TargetingState> { }
    }
}