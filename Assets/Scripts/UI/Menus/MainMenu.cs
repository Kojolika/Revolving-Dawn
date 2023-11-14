using System;
using Data;
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
        }

        void OpenSettings()
        {
        }

        void QuitGame()
        {
            
        }
    }
}