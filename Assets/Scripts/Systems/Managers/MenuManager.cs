using UnityEngine;
using System.Collections.Generic;
using UI;

namespace Systems.Managers
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager StaticInstance { get; private set; }
        public static Stack<GameObject> MenuStack = new Stack<GameObject>();

        [SerializeField] GameObject pauseBackGround;
        private GameObject pauseBackGroundInstance;

        //MENUS
        public Menu escapeMenu;
        public Menu SettingsMenu;
        public Menu DeckViewerMenu;


        private void Awake()
        {
            if (StaticInstance == null)
            {
                StaticInstance = this;
                pauseBackGroundInstance = Instantiate(pauseBackGround, transform);
                pauseBackGroundInstance.SetActive(false);
            }
            else
            {
                Destroy(this);
            }
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (MenuStack.Count < 1)
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
            var instance = Instantiate(menu, transform);

            if (MenuStack.Count > 0)
            {
                instance.GetComponent<Canvas>().sortingOrder = MenuStack.Peek().GetComponent<Canvas>().sortingOrder + 1;
                MenuStack.Peek().gameObject.SetActive(false);
            }
            else
            {
                pauseBackGroundInstance.SetActive(true);
                instance.GetComponent<Canvas>().sortingOrder = pauseBackGroundInstance.GetComponent<Canvas>().sortingOrder + 1;

                FightInput.PlayerTurnInputManager.StaticInstance.isPaused = true;
            }

            instance.GetComponent<Menu>().HandleInput(input);
            MenuStack.Push(instance);
        }
        public void CloseMenu()
        {
            var menu = MenuStack.Pop();
            Destroy(menu);

            if (MenuStack.Count > 0)
                MenuStack.Peek().gameObject.SetActive(true);
            else
            {
                pauseBackGroundInstance.SetActive(false);
                FightInput.PlayerTurnInputManager.StaticInstance.isPaused = false;
            }
        }
    }
}
