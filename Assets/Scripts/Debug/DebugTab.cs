#if !PRODUCTION || ENABLE_DEBUG_MENU
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Views.Common;

namespace Koj.Debug
{
    public class DebugTab : MonoBehaviour, IView<string>
    {
        [SerializeField]
        private TextMeshProUGUI textMeshPro;

        private DebugMenu debugMenu;

        private string path;

        [Zenject.Inject]
        private void Construct(DebugMenu debugMenu)
        {
            this.debugMenu = debugMenu;
        }

        public void Populate(string data)
        {
            path = data;
            string tabText = data.EndsWith(DebugMenu.PageExtension)
                ? data[..^DebugMenu.PageExtension.Length]
                : data;
            textMeshPro.text = tabText;
        }

        public void OpenPage()
        {
            debugMenu.OpenAtPath(path);
        }
    }
}
#endif