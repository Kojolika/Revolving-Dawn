using System.Threading;
using Cysharp.Threading.Tasks;
using Fight;
using Tooling.Logging;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Views;

namespace Controllers
{
    public class PlayerHandInputController
    {
        public PlayerInputState CurrentState { get; private set; }
        private CancellationToken cancellationToken;

        public PlayerHandInputController(InputActionAsset playerHandInputActionAsset, PlayerHandView playerHandView, DefaultState defaultState)
        {
            playerHandInputActionAsset.FindActionMap("PlayerHand").Enable();
            cancellationToken = playerHandView.GetCancellationTokenOnDestroy();

            if (Touchscreen.current != null && !EnhancedTouchSupport.enabled)
            {
                EnhancedTouchSupport.Enable();
            }

            SetNextState(defaultState);
            _ = Update();
        }

        private async UniTask Update()
        {
            while (!cancellationToken.IsCancellationRequested)
            { 
                CurrentState.Tick();

                await UniTask.Yield(PlayerLoopTiming.Update);

                if (CurrentState.NextState != null)
                {
                    SetNextState(CurrentState.NextState);
                }
            }
        }
        private void SetNextState(PlayerInputState nextState)
        {
            CurrentState?.OnExit();
            CurrentState = nextState;
            CurrentState.OnEnter();
        }
    }
}