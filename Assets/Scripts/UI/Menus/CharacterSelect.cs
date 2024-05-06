using System.Collections.Generic;
using Data.Definitions.Player;
using UI.Menus.Common;
using Utils.Attributes;
using UnityEngine;
using UI.Common.DisplayElements;
using Tooling.Logging;
using UI.Common;
using Systems.Map;
using Systems.Managers;
using Settings;
using Models.Player;

namespace UI.Menus
{
    public class CharacterSelect : Menu<CharacterSelect.Data>
    {
        public class Data
        {
            public List<PlayerClassDefinition> Classes;

            public Data(List<PlayerClassDefinition> classes)
            {
                Classes = classes;
            }
        }

        [ResourcePath]
        public string ResourcePath => nameof(CharacterSelect);

        [SerializeField] ClassDisplayElement classDisplayElementPrefab;
        [SerializeField] Transform classDisplayListRoot;
        [SerializeField] MyButton playButton;

        List<ClassDisplayElement> classDisplayElements = new();
        PlayerClassDefinition selectedclass;
        MenuManager menuManager;
        PlayerDataManager playerDataManager;
        MapSettings mapSettings;

        [Zenject.Inject]
        void Construct(MenuManager menuManager, PlayerDataManager playerDataManager, MapSettings mapSettings)
        {
            this.menuManager = menuManager;
            this.playerDataManager = playerDataManager;
            this.mapSettings = mapSettings;
        }

        public override void Populate(Data data)
        {
            classDisplayElements.Clear();
            foreach (var classDef in data.Classes)
            {
                var newClassDisplayElement = Instantiate(classDisplayElementPrefab, classDisplayListRoot);
                classDisplayElements.Add(newClassDisplayElement);
                newClassDisplayElement.Populate(classDef);
                newClassDisplayElement.gameObject.SetActive(true);

                newClassDisplayElement.SelectButton.ClearEventListeners();
                newClassDisplayElement.SelectButton.Pressed += () =>
                {
                    playButton.gameObject.SetActive(true);
                    selectedclass = classDef;
                };
            }

            playButton.gameObject.SetActive(false);
            playButton.ClearEventListeners();
            playButton.Pressed += SaveSelectionAndGenerateRun;
        }

        async void SaveSelectionAndGenerateRun()
        {
            MyLogger.Log("Generating map...");
            await playerDataManager.StartNewRun(selectedclass);
            _ = menuManager.Open<MapView, MapView.Data>(
                new MapView.Data()
                {
                    MapDefinition = playerDataManager.CurrentPlayerDefinition.CurrentRun.CurrentMap
                }
            );
        }
    }
}