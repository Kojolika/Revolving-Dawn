using UnityEngine;
using UnityEngine.InputSystem;
using Views;

namespace Controllers
{
    public class PlayerHandInputController
    {
        private readonly InputActionAsset playerHandInputActionAsset;
        private readonly InputActionMap playerHandInputActionMap;
        private readonly InputAction hoverAction;
        private readonly Camera handViewCamera;

        public PlayerHandInputController(InputActionAsset playerHandInputActionAsset, PlayerHandView playerHandView)
        {
            this.playerHandInputActionAsset = playerHandInputActionAsset;
            this.playerHandInputActionMap = playerHandInputActionAsset.FindActionMap("PlayerHand");
            this.hoverAction = playerHandInputActionMap.FindAction("hoverCard");
            this.handViewCamera = playerHandView.Camera;
        }
    }
}