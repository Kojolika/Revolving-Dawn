using UnityEngine;

namespace UI
{
    public class GlobalUIEvents : MonoBehaviour
    {
        //Need to init this to a class after loading the main menu
        //Create new one for each scene? or make it presisten
        //For now adding it to the fightmanager GO
        public static GlobalUIEvents staticInstance;

        void Awake()
        {
            if (staticInstance == null)
                staticInstance = this;
            else
                Destroy(this);
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                TriggerPausedGame();
            }
        }

        //Can use this for all keybindings in the future
        public delegate void PausedGame();
        public static event PausedGame OnPausedGame;
        void TriggerPausedGame()
        {
            if (OnPausedGame != null)
            {
                OnPausedGame();
            }
        }
    }
}
