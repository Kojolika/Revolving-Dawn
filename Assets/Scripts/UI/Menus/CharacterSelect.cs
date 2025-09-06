using System.Collections.Generic;
using UI.Menus.Common;
using Utils.Attributes;
using UnityEngine;
using UI.Common.DisplayElements;
using Tooling.Logging;
using UI.Common;
using Systems.Managers;
using Tooling.StaticData.Data;
using UnityEngine.Serialization;
using Views.Common;

namespace UI.Menus
{
    public class CharacterSelect : Menu<CharacterSelect.Data>
    {
        public class Data
        {
            public List<PlayerClass> Classes;
        }

        [ResourcePath]
        public string ResourcePath => nameof(CharacterSelect);


        [SerializeField] private Transform classDisplayListRoot;
        [SerializeField] private MyButton  playButton;

        private List<PlayerClassView> classDisplayElements = new();
        private PlayerClass           selectedClass;
        private MenuManager           menuManager;
        private PlayerDataManager     playerDataManager;
        private MapSettings           mapSettings;
        private ViewFactory           viewFactory;

        [Zenject.Inject]
        private void Construct(MenuManager menuManager, PlayerDataManager playerDataManager, MapSettings mapSettings, ViewFactory viewFactory)
        {
            this.menuManager       = menuManager;
            this.playerDataManager = playerDataManager;
            this.mapSettings       = mapSettings;
            this.viewFactory       = viewFactory;
        }

        public override void Populate(Data data)
        {
            classDisplayElements.Clear();
            foreach (var classDef in data.Classes)
            {
                var newClassDisplayElement = viewFactory.Create<PlayerClassView, PlayerClass>(classDef);
                classDisplayElements.Add(newClassDisplayElement);
                newClassDisplayElement.Populate(classDef);
                newClassDisplayElement.gameObject.SetActive(true);

                newClassDisplayElement.SelectButton.ClearEventListeners();
                newClassDisplayElement.SelectButton.Pressed += () =>
                {
                    playButton.gameObject.SetActive(true);
                    selectedClass = classDef;
                };
            }

            playButton.gameObject.SetActive(false);
            playButton.ClearEventListeners();
            playButton.Pressed += SaveSelectionAndGenerateRun;
        }

        private async void SaveSelectionAndGenerateRun()
        {
            MyLogger.Info("Generating map...");
            await playerDataManager.StartNewRun(selectedClass);
            _ = menuManager.Open<MapView, MapView.Data>(
                new MapView.Data()
                {
                    MapDefinition = playerDataManager.CurrentPlayerDefinition.CurrentRun.CurrentMap
                }
            );
        }
    }
}