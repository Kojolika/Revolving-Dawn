using UnityEngine.InputSystem;
using Views;
using Zenject;

namespace Fight.Input
{
    public class DraggingState : PlayerInputState
    {
        private readonly CardView                                       cardView;
        private readonly DefaultState                                   defaultState;
        private readonly Tooling.StaticData.Data.PlayerHandViewSettings playerHandInputSettings;
        private readonly TargetingState.Factory                         targetingStateFactory;

        public DraggingState(InputActionAsset              playerHandInputActionAsset,
            PlayerHandView                                 playerHandView,
            CardView                                       cardView,
            DefaultState                                   defaultState,
            Tooling.StaticData.Data.PlayerHandViewSettings playerHandInputSettings,
            TargetingState.Factory                         targetingStateFactory)
            : base(playerHandInputActionAsset, playerHandView)
        {
            this.cardView = cardView;
            this.defaultState = defaultState;
            this.playerHandInputSettings = playerHandInputSettings;
            this.targetingStateFactory = targetingStateFactory;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            playerHandView.UnsetSelectionEffects(cardView);
        }

        public override void Update()
        {
            if (!dragAction.inProgress)
            {
                NextState = defaultState;
                return;
            }

            var playerInputScreenPosition = dragAction.ReadValue<UnityEngine.Vector2>();
            var playerInputViewPortPosition = playerHandView.Camera.ScreenToViewportPoint(playerInputScreenPosition);
            if (playerInputViewPortPosition.y > playerHandInputSettings.PositionOnScreenWhereTargetingStarts)
            {
                NextState = targetingStateFactory.Create(cardView);
                return;
            }

            var playerInputWorldPosition = playerHandView.Camera.ScreenToWorldPoint(new UnityEngine.Vector3(playerInputScreenPosition.x, playerInputScreenPosition.y));
            cardView.transform.position = new UnityEngine.Vector3(playerInputWorldPosition.x,
                playerInputWorldPosition.y,
                cardView.transform.position.z
            );
        }

        public class Factory : PlaceholderFactory<CardView, DraggingState> { }
    }
}