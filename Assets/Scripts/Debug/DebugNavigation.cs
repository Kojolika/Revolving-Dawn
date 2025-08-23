using System.Text;
using TMPro;
using Tooling.Components;
using UnityEngine;

namespace Koj.Debug
{
    public class DebugNavigation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tmp;
        [SerializeField] private DebugMenu       debugMenu;
        [SerializeField] private TMPLinkHandler  linkHandler;

        private void Awake()
        {
            debugMenu.OnPathOpened    += OnPathOpened;
            linkHandler.OnLinkClicked += OnLinkClicked;
        }

        private void OnPathOpened(string path)
        {
            var tokens = path.Split('/');

            // What we display on the nav bar, includes links
            var displayStringBuilder = new StringBuilder();

            // The actual path used to incrementally build the link IDs
            var navPathStringBuilder = new StringBuilder();

            for (int i = 0; i < tokens.Length; i++)
            {
                navPathStringBuilder.Append(tokens[i]);

                // Home folder
                if (i == 0 && tokens[i] == string.Empty)
                {
                    displayStringBuilder.Append("<link=\"\">..</link>");
                }
                else
                {
                    displayStringBuilder.Append($"<link=\"{navPathStringBuilder}\">{tokens[i]}</link>");
                }

                if (i != tokens.Length - 1)
                {
                    displayStringBuilder.Append('/');
                    navPathStringBuilder.Append('/');
                }
            }

            tmp.text = displayStringBuilder.ToString();
        }

        private void OnLinkClicked((string id, string text) linkInfo)
        {
            debugMenu.OpenAtPath(linkInfo.id);
        }

        private void OnDestroy()
        {
            if (debugMenu != null)
            {
                debugMenu.OnPathOpened -= OnPathOpened;
            }

            if (linkHandler != null)
            {
                linkHandler.OnLinkClicked -= OnLinkClicked;
            }
        }
    }
}