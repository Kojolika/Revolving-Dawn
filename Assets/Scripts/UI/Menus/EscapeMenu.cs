using UnityEngine;
using UI.Common;
using UI.Menus.Common;
using Utils.Attributes;

namespace UI.Menus
{
    public class EscapeMenu : Menu<Data.Null>
    {
        [ResourcePath]
        public string ResourcePath => nameof(EscapeMenu);
        
        [Header("Buttons")] 
        [SerializeField] private MyButton resumeButton;
        [SerializeField] private MyButton settingsButton;
        [SerializeField] private MyButton mainMenuButton;
        [SerializeField] private MyButton closeGameButton;

        public override void Populate(Data.Null data)
        {
            resumeButton.Pressed += Close;
            mainMenuButton.Pressed += MainMenu;
            closeGameButton.Pressed += ExitToDesktop;
        }

        public void MainMenu()
        {
            // Save Game,
            // Load Main Menu
        }

        public void ExitToDesktop()
        {
            // Save Game,
            // Close Game
        }
    }
}