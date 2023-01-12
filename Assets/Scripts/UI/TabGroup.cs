using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UI
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
            if (tabs == null)
            {
                tabs = new List<TabButton>();
            }
            if(tabs.Count == 0) OnTabSelected(button);
            
            tabs.Add(button);
        }

        public void OnTabEnter(TabButton button)
        {
            ResetTabs();
            if (!selectedTab || selectedTab != button)
                button.background.sprite = hovered;
        }
        public void OnTabExit(TabButton button)
        {
            ResetTabs();
        }
        public void OnTabSelected(TabButton button)
        {
            Debug.Log("calling select");
            if (selectedTab) selectedTab.menuOfTab.SetActive(false); //disable previous tab
            selectedTab = button;
            ResetTabs();
            button.background.sprite = selected;
            button.menuOfTab.SetActive(true);
        }
        public void ResetTabs()
        {
            foreach (var button in tabs)
            {
                if (selectedTab == button) continue;
                button.background.sprite = idle;
            }
        }
    }
}
