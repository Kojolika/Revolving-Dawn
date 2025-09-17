using System.Collections.Generic;
using Tooling.StaticData.Data;
using UnityEngine;
using Views.Common;

namespace Views
{
    public class ManaPoolView : MonoBehaviour, IView<List<(Mana, long)>>
    {
        [SerializeField] private PlayerManaView playerManaViewPrefab;
        [SerializeField] private Transform      contentAnchor;

        private CharacterSettings      characterSettings;
        private List<PlayerManaView>   manaPool;
        private ViewListFactory        viewListFactory;
        private ViewList<(Mana, long)> viewList;

        [Zenject.Inject]
        private void Construct(CharacterSettings characterSettings, ViewListFactory viewListFactory)
        {
            this.characterSettings = characterSettings;
            this.viewListFactory   = viewListFactory;
        }

        public void Start()
        {
            var manaList = new List<(Mana, long)>();
            foreach (var mana in characterSettings.AllManaTypesAvailable)
            {
                manaList.Add((mana, 0));
            }

            Populate(manaList);
        }

        public void Populate(List<(Mana, long)> data)
        {
            viewList ??= viewListFactory.Create(data, CreateView, contentAnchor);
            viewList.Populate(data);
        }

        private IView<(Mana, long)> CreateView()
        {
            return Instantiate(playerManaViewPrefab, contentAnchor);
        }
    }
}