using System.Threading;
using Cysharp.Threading.Tasks;
using Fight.Input;
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
            _ = RunUpdateLoop();
        }

        private async UniTask RunUpdateLoop()
        {
            while (!cancellationToken.IsCancellationRequested)
            { 
                MyLogger.Log($"State : {CurrentState}");
                CurrentState.Update();

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