using System;
using System.Collections.Generic;
using Models.Mana;
using Tooling.StaticData.Data;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Views.Common;

namespace Views
{
    public class ManaPoolView : MonoBehaviour, IView<List<(ManaModel, long)>>
    {
        [SerializeField] private PlayerManaView playerManaViewPrefab;
        [SerializeField] private Transform contentAnchor;

        private CharacterSettings characterSettings;
        private List<PlayerManaView> manaPool;
        private ViewListFactory viewListFactory;
        private ViewList<(ManaModel, long)> viewList;

        [Zenject.Inject]
        private void Construct(CharacterSettings characterSettings, ViewListFactory viewListFactory)
        {
            this.characterSettings = characterSettings;
            this.viewListFactory = viewListFactory;
        }

        public void Start()
        {
            var manaList = new List<(ManaModel, long)>();
            foreach (var mana in characterSettings.AllManaTypesAvailable)
            {
                manaList.Add((mana.Representation, 0));
            }

            Populate(manaList);
        }

        public void Populate(List<(ManaModel, long)> data)
        {
            viewList ??= viewListFactory.Create(data, CreateView, contentAnchor);
            viewList.Populate(data);
        }

        private IView<(ManaModel, long)> CreateView() => Instantiate(playerManaViewPrefab, contentAnchor);
    }
}