using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UI
{
    public class EscapeMenu : MonoBehaviour
    {
        public EscapeMenu staticInstance;
        public List<MenuButton> buttons;

        public enum ButtonType
        {
            Resume,
            Settings,
            MainMenu,
            Desktop
        }

        [SerializeField] Sprite hovered;
        [SerializeField] Sprite selected;
        [SerializeField] Sprite idle;

        private void Awake() {
            if(!staticInstance)
                staticInstance = this;
            else
                Destroy(this);
        }
        public void Subscribe(MenuButton button)
        {
            if (buttons == null)
            {
                buttons = new List<MenuButton>();
            }

            buttons.Add(button);
        }

        public void OnButtonEnter(MenuButton button)
        {
            ResetButtons();
            button.background.sprite = hovered;
        }
        public void OnButtonExit(MenuButton button)
        {
            ResetButtons();
        }
        public void OnButtonSelected(MenuButton button)
        {
            ResetButtons();
            button.background.sprite = selected;

            switch(button.buttonType)
            {
                case ButtonType.Resume:
                    Overlay.staticInstance.PauseOrUnPause();
                    break;
                case ButtonType.Settings:
                    button.menuOfTab.SetActive(true);
                    this.gameObject.SetActive(false);
                    break;
                case ButtonType.MainMenu:
                    MainMenu();
                    break;
                case ButtonType.Desktop:
                    ExitToDesktop();
                    break;
            }
            
            
        }
        public void ResetButtons()
        {
            foreach (var button in buttons)
            {
                button.background.sprite = idle;
            }
        }

        public void MainMenu()
        {
            //Save Game,
            //Load Main Menu
        }
        public void ExitToDesktop()
        {
            //Save Game,
            //Close Game
        }
    }
}
