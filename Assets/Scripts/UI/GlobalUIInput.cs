using UnityEngine;

namespace UI
{
    public class GlobalUIInput : MonoBehaviour
    {
        //Need to init this to a class after loading the main menu
        //Create new one for each scene? or make it presisten
        //For now adding it to the fightmanager GO
        public static GlobalUIInput staticInstance;

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
                TriggerEscapeKeyPressed();
            }
        }

        //Can use this for all keybindings in the future
        public delegate void EscapeKeyPressed();
        public static event EscapeKeyPressed OnEscapeKeyPressed;
        void TriggerEscapeKeyPressed()
        {
            if (OnEscapeKeyPressed != null)
            {
                OnEscapeKeyPressed();
            }
        }
    }
}
