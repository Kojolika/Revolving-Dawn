using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Overlay : MonoBehaviour
    {
        public static Overlay staticInstance;
        [SerializeField] GameObject pauseScreen;
        bool isFightPaused = false;
        bool IsFightPaused { get => isFightPaused; }
        private void Awake()
        {
            if (!staticInstance)
                staticInstance = this;
            else
                Destroy(this);

            GlobalUIInput.OnEscapeKeyPressed += PauseOrUnPause;
        }
        public void PauseOrUnPause()
        {
            //set all children of pauseScreen to inactive
            //unless child == escape menu, activate escape menu
            pauseScreen.SetActive(!isFightPaused);
            fightInput.PlayerTurnInputManager.staticInstance.isEnabled = isFightPaused;
            isFightPaused = !isFightPaused;
        }
    }
}
