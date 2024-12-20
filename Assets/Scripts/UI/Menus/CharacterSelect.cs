﻿using System.Collections.Generic;
using UI.Menus.Common;
using Utils.Attributes;
using UnityEngine;
using UI.Common.DisplayElements;
using Tooling.Logging;
using UI.Common;
using Systems.Managers;
using Settings;
using Models.Player;

namespace UI.Menus
{
    public class CharacterSelect : Menu<CharacterSelect.Data>
    {
        public class Data
        {
            public List<PlayerClassSODefinition> Classes;
        }

        [ResourcePath]
        public string ResourcePath => nameof(CharacterSelect);

        [SerializeField] ClassDisplayElement classDisplayElementPrefab;
        [SerializeField] Transform classDisplayListRoot;
        [SerializeField] MyButton playButton;

        List<ClassDisplayElement> classDisplayElements = new();
        PlayerClassSODefinition selectedClass;
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
                    selectedClass = classDef;
                };
            }

            playButton.gameObject.SetActive(false);
            playButton.ClearEventListeners();
            playButton.Pressed += SaveSelectionAndGenerateRun;
        }

        async void SaveSelectionAndGenerateRun()
        {
            MyLogger.Log("Generating map...");
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