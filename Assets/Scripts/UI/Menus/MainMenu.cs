using System.Collections.Generic;
using Data;
using Models.Player;
using Systems.Managers;
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
        private MySceneManager mySceneManager;
        private MenuManager menuManager;
        private List<PlayerClassSODefinition> playerClassDefinitions;

        [Zenject.Inject]
        async void Construct(
            PlayerDataManager playerDataManager,
            MySceneManager mySceneManager,
            MenuManager menuManager,
            StaticDataReference<PlayerClassSODefinition> playerClassReferences)
        {
            this.playerDataManager = playerDataManager;
            this.mySceneManager = mySceneManager;
            this.menuManager = menuManager;
            playerClassDefinitions = await playerClassReferences.LoadAssetsAsync();
        }

        private void Awake()
        {
            playButton.Pressed += StartNewGameOrLoadCurrent;
            settingsButton.Pressed += OpenSettings;
            quitButton.Pressed += QuitGame;
        }

        public override void Populate(Null data) { }

        async void StartNewGameOrLoadCurrent()
        {
            // If continuing load current fight or load current map
            // otherwise open character selection

            var currentRun = playerDataManager.CurrentPlayerDefinition.CurrentRun;
            if (currentRun == null)
            {
                _ = menuManager.Open<CharacterSelect, CharacterSelect.Data>(
                    new CharacterSelect.Data() { Classes = playerClassDefinitions }
                );
            }
            else if (currentRun.CurrentFight != null)
            {
                await mySceneManager.LoadScene(MySceneManager.SceneIndex.Fight);
            }
            else if (currentRun.CurrentMap != null)
            {
                _ = menuManager.Open<MapView, MapView.Data>(
                    new MapView.Data()
                    {
                        MapDefinition = currentRun.CurrentMap,
                        CurrentNode = currentRun.CurrentMapNode
                    }
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