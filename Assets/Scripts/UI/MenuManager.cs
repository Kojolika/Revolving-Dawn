using UnityEngine;
using System.Dynamic;
using System.Collections.Generic;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager staticInstance;
        public static Stack<GameObject> MenuStack = new Stack<GameObject>();

        [SerializeField] GameObject pauseBackGround;
        private GameObject pauseBackGroundInstance;
        
        //MENUS
        public Menu escapeMenu;
        public Menu SettingsMenu;
        public Menu DeckViewerMenu;

    
        private void Awake() {
            if(staticInstance == null)
            {
                staticInstance = this;
                pauseBackGroundInstance = Instantiate(pauseBackGround,transform);
                pauseBackGroundInstance.SetActive(false);
            }
            else
            {
                Destroy(this);
            }
        }
        void Update() 
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(MenuStack.Count < 1)
                {
                    escapeMenu.GetComponentInChildren<Menu>().Open();
                }
                else
                {
                    MenuStack.Peek().GetComponentInChildren<Menu>().Close();
                }

            }
        }
        public void OpenMenu(GameObject menu, dynamic input = null)
        {
             var instance = Instantiate(menu,transform);

            if(MenuStack.Count > 0)
            {
                instance.GetComponent<Canvas>().sortingOrder = MenuStack.Peek().GetComponent<Canvas>().sortingOrder + 1;
                MenuStack.Peek().gameObject.SetActive(false);
            }
            else
            {
                pauseBackGroundInstance.SetActive(true);
                instance.GetComponent<Canvas>().sortingOrder = pauseBackGroundInstance.GetComponent<Canvas>().sortingOrder + 1;

                fightInput.PlayerTurnInputManager.staticInstance.isPaused = true;
            }
            
            instance.GetComponent<Menu>().HandleInput(input);
            MenuStack.Push(instance);     
        }
        public void CloseMenu()
        {
            var menu = MenuStack.Pop();
            Destroy(menu);

            if(MenuStack.Count > 0)
                MenuStack.Peek().gameObject.SetActive(true);
            else
            {
                pauseBackGroundInstance.SetActive(false);
                fightInput.PlayerTurnInputManager.staticInstance.isPaused = false;
            }
        }

        public delegate void PausedInput(bool value);
        public static event PausedInput OnPausedInput;
        static void TriggerPausedInput(bool value)
        {
            if (OnPausedInput != null)
            {
                OnPausedInput(value);
            }
        }
    }
}
