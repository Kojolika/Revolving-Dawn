using System.Numerics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using Views;
using Zenject;

namespace Fight
{
    public class DraggingState : PlayerInputState
    {
        private readonly CancellationTokenSource cts;
        private readonly CardView cardView;

        public DraggingState(InputActionAsset playerHandInputActionAsset,
            PlayerHandView playerHandView,
            CardView cardView)
            : base(playerHandInputActionAsset, playerHandView)
        {
            cts = new();
            this.cardView = cardView;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _ = CardFollowPlayerInputTask(cardView);
        }

        public override void OnExit()
        {
            base.OnExit();
            cts.Cancel();
            cts.Dispose();
        }

        private async UniTask CardFollowPlayerInputTask(CardView cardView)
        {
            while (!cts.Token.IsCancellationRequested)
            {
                var playerInputLocation = dragAction.ReadValue<Vector2>();
                var playerInputWorldSpace = playerHandView.Camera.ScreenToWorldPoint(new UnityEngine.Vector3(playerInputLocation.X,playerInputLocation.Y));
                cardView.transform.position = new UnityEngine.Vector3(playerInputWorldSpace.x,
                    playerInputWorldSpace.y,
                    cardView.transform.position.z
                );
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }

        public class Factory : PlaceholderFactory<CardView, DraggingState> { }
    }
}