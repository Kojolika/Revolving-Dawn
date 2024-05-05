﻿using Data;
using Models.Player;
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
        private MenuManager menuManager;
        private StaticDataManager staticDataManager;

        [Zenject.Inject]
        void Construct(PlayerDataManager playerDataManager, MenuManager menuManager, StaticDataManager staticDataManager)
        {
            this.playerDataManager = playerDataManager;
            this.menuManager = menuManager;
            this.staticDataManager = staticDataManager;
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

            var currentRun = playerDataManager.CurrentPlayerDefinition.CurrentRun;
            if (currentRun == null)
            {
                _ = menuManager.Open<CharacterSelect, CharacterSelect.Data>(
                    new CharacterSelect.Data(
                        staticDataManager.GetAllAssetsForType<PlayerClassDefinition>()
                    )
                );
            }
            // TODO: add if current fight is not null load fight
            else if (currentRun.CurrentMap != null)
            {   
                _ = menuManager.Open<MapView, MapView.Data>(new MapView.Data() { MapDefinition = currentRun.CurrentMap });
            }
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