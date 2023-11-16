using System;
using Data;
using Systems.Managers;
using UI.Common;
using UI.Menus.Common;
using UnityEngine;
using Utils.Attributes;

namespace UI.Menus
{
    public class MainMenu : Menu<Data.Null>
    {
        [ResourcePath] public static string ResourcePath => nameof(MainMenu);

        [SerializeField] private MyButton playButton;
        [SerializeField] private MyButton settingsButton;
        [SerializeField] private MyButton quitButton;

        private PlayerDataManager playerDataManager;

        [Zenject.Inject]
        void Construct(PlayerDataManager playerDataManager)
        {
            this.playerDataManager = playerDataManager;
        }

        private void Awake()
        {
            playButton.Pressed += StartNewGameOrLoadCurrent;
            settingsButton.Pressed += OpenSettings;
            quitButton.Pressed += QuitGame;
        }

        public override void Populate(Null data)
        {
            // no data??
        }

        void StartNewGameOrLoadCurrent()
        {
            // If continuing load current fight or load current map
            // otherwise open character selection
            if (playerDataManager.CurrentPlayerDefinition.CurrentRun == null)
            {
                // open character selection
            }
            
            // otherwise continue run
        }

        void OpenSettings()
        {
        }

        void QuitGame()
        {
            Application.Quit();
        }
    }
}