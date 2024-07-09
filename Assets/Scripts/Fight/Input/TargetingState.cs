
using System.Linq;
using Fight.Input;
using Settings;
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

        private bool shouldDrawCurve = false;

        public TargetingState(InputActionAsset playerHandInputActionAsset,
            PlayerHandView playerHandView,
            CardView cardView,
            DefaultState defaultState,
            TargetingArrowView targetingArrowView,
            PlayerHandViewSettings playerHandViewSettings)
            : base(playerHandInputActionAsset, playerHandView)
        {
            this.cardView = cardView;
            this.defaultState = defaultState;
            this.targetingArrowView = targetingArrowView;
            this.playerHandViewSettings = playerHandViewSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            shouldDrawCurve = cardView.Model.PlayEffects.Any(effect => effect.Targeting == Cards.Targeting.Options.Enemy);
            targetingArrowView.SetActive(shouldDrawCurve);
        }

        public override void OnExit()
        {
            targetingArrowView.SetActive(false);
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