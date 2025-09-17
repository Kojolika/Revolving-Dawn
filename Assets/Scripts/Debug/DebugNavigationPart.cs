using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Koj.Debug
{
    /// <summary>
    /// Represents a part of a path on the debug menu.
    /// This part is clickable to be able to navigate to the folder that this part represents.
    /// </summary>
    public class DebugNavigationPart : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tmp;
        [SerializeField] private Button          button;

        private DebugMenu debugMenu;
        private string    path;

        [Inject]
        private void Construct(DebugMenu debugMenu, string path, bool clickable, bool isHome)
        {
            this.debugMenu      = debugMenu;
            this.path           = path;
            button.interactable = clickable;
            
            tmp.text = isHome
                ? "home"
                : path;
        }

        public void OpenPath()
        {
            debugMenu.OpenAtPath(path);
        }

        public class Factory : PlaceholderFactory<string, bool, bool, Transform, DebugNavigationPart>
        {
            
        }

        public class CustomFactory : IFactory<string, bool, bool, Transform, DebugNavigationPart>
        {
            private DiContainer         diContainer;
            private DebugNavigationPart navPartPrefab;

            [Inject]
            private void Construct(DiContainer diContainer, DebugNavigationPart navPartPrefab)
            {
                this.diContainer   = diContainer;
                this.navPartPrefab = navPartPrefab;
            }

            public DebugNavigationPart Create(string path, bool clickable, bool isHome, Transform parent)
            {
                return diContainer.InstantiatePrefabForComponent<DebugNavigationPart>(navPartPrefab, parent, extraArgs: new object[] { path, clickable, isHome });
            }
        }
    }
}