using System.Collections.Generic;
using Data;
using Systems.Managers;
using Tooling.StaticData.Data;
using UI.Common;
using UI.Menus.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        private MySceneManager    mySceneManager;
        private MenuManager       menuManager;

        [Zenject.Inject]
        private void Construct(PlayerDataManager playerDataManager, MySceneManager mySceneManager, MenuManager menuManager)
        {
            this.playerDataManager = playerDataManager;
            this.mySceneManager    = mySceneManager;
            this.menuManager       = menuManager;
        }

        private void Awake()
        {
            playButton.Pressed     += StartNewGameOrLoadCurrent;
            settingsButton.Pressed += OpenSettings;
            quitButton.Pressed     += QuitGame;
        }

        public override void Populate(Null data)
        {
        }

        private async void StartNewGameOrLoadCurrent()
        {
            // If continuing load current fight or load current map
            // otherwise open character selection


            var currentRun = playerDataManager.CurrentPlayerDefinition.CurrentRun;
            if (currentRun == null)
            {
                _ = menuManager.Open<CharacterSelect, CharacterSelect.Data>(
                    new CharacterSelect.Data { Classes = StaticDatabase.Instance.GetInstancesForType<PlayerClass>() }
                );
            }
            else if (currentRun.CurrentFight != null)
            {
                await mySceneManager.LoadScene(MySceneManager.SceneIndex.Fight);
            }
            else if (currentRun.CurrentMap != null)
            {
                _ = menuManager.Open<MapView, MapView.Data>(
                    new MapView.Data { MapDefinition = currentRun.CurrentMap }
                );
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