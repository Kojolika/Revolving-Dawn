using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MyButton
    {
        /// <summary>
        /// The group this tab button is part of.
        /// </summary>
        public TabGroup tabGroup;

        /// <summary>
        /// The GameObject associated with opening when this <see cref="TabButton"/> is selected.
        /// </summary>
        internal GameObject menuOfTab;

        private void Start()
        {
            tabGroup.Subscribe(this);

            shouldHandleOwnImage = false;

            Pressed += () => tabGroup.OnTabSelected(this);
            Entered += () => tabGroup.OnTabEnter(this);
            Exited += () => tabGroup.OnTabExit(this);
        }

        public void SetGameObjectForTab(GameObject go) => menuOfTab = go;
    }
}