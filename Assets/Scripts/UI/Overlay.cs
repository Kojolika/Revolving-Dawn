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
            if(!isFightPaused)
            {
                bool bothSet = false;
                foreach(Transform child in pauseScreen.transform)
                {
                    if(!bothSet && child.TryGetComponent<EscapeMenu>(out EscapeMenu menu))
                    {
                        menu.gameObject.SetActive(true);
                        bothSet = true;
                    }
                    else if(child.TryGetComponent<TabGroup>(out TabGroup tabGroup))
                    {
                        tabGroup.gameObject.SetActive(false);
                        break;
                    }
                }
            }
            fightInput.PlayerTurnInputManager.staticInstance.isEnabled = isFightPaused;
            isFightPaused = !isFightPaused;
        }
    }
}
