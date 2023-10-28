using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UI.Common
{
    public class TabGroup : MonoBehaviour
    {
        public List<TabButton> tabs;
        public TabButton selectedTab;

        [SerializeField] Sprite hovered;
        [SerializeField] Sprite selected;
        [SerializeField] Sprite idle;


        public void Subscribe(TabButton button)
        {
            tabs ??= new List<TabButton>();

            if (tabs.Count == 0)
            {
                OnTabSelected(button);
            }

            tabs.Add(button);
        }

        public void OnTabEnter(TabButton button)
        {
            ResetTabs();
            if (!selectedTab || selectedTab != button)
            {
                button.SetImage(hovered);
            }
        }

        public void OnTabExit(TabButton button)
        {
            ResetTabs();
        }

        public void OnTabSelected(TabButton button)
        {
            if (selectedTab && selectedTab.menuOfTab != null)
            {
                // disable previous tab
                selectedTab.menuOfTab.SetActive(false);
            }

            selectedTab = button;
            ResetTabs();

            button.SetImage(selected);

            if (button.menuOfTab != null)
            {
                button.menuOfTab.SetActive(true);
            }
        }

        private void ResetTabs()
        {
            foreach (var tab in tabs.Where(tab => tab != selectedTab))
            {
                tab.SetImage(idle);
            }
        }
    }
}