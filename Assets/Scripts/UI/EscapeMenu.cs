using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Dynamic;

namespace UI
{
    public class EscapeMenu : Menu
    {
        public EscapeMenu staticInstance;
        public List<EscapeMenuButton> buttons;
        [SerializeField] Sprite hovered;
        [SerializeField] Sprite selected;
        [SerializeField] Sprite idle;

        public override ExpandoObject MenuInput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public enum ButtonType
        {
            Resume,
            Settings,
            MainMenu,
            Desktop
        }
        private void Awake()
        {
            if (!staticInstance)
                staticInstance = this;
            else
                Destroy(this);
        }
        public void Subscribe(EscapeMenuButton button)
        {
            if (buttons == null)
            {
                buttons = new List<EscapeMenuButton>();
            }

            buttons.Add(button);
        }

        public void OnButtonEnter(EscapeMenuButton button)
        {
            ResetButtons();
            button.background.sprite = hovered;
        }
        public void OnButtonExit(EscapeMenuButton button)
        {
            ResetButtons();
        }
        public void OnButtonSelected(EscapeMenuButton button)
        {
            ResetButtons();
            button.background.sprite = selected;

            switch (button.buttonType)
            {
                case ButtonType.Resume:
                    MenuManager.StaticInstance.escapeMenu.Close();
                    break;
                case ButtonType.Settings:
                    MenuManager.StaticInstance.SettingsMenu.Open();
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
        void OnEnable()
        {
            ResetButtons();
        }

        public override void HandleInput(dynamic input)
        {

        }
    }
}
