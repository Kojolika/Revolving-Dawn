using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tooling.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPLinkHandler : MonoBehaviour, IPointerClickHandler
    {
        private TextMeshProUGUI tmp;

        private TextMeshProUGUI Tmp
        {
            get
            {
                if (tmp == null)
                {
                    tmp = GetComponent<TextMeshProUGUI>();
                }

                return tmp;
            }
        }

        public event Action<(string id, string text)> OnLinkClicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            // TODO: Create UI Actions with pointer hover? Instead of using hold input
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(Tmp, UnityEngine.Input.mousePosition, null);
            if (linkIndex == -1)
            {
                return;
            }

            TMP_LinkInfo linkInfo = Tmp.textInfo.linkInfo[linkIndex];
            OnLinkClicked?.Invoke((linkInfo.GetLinkID(), linkInfo.GetLinkText()));
        }

        private void OnDestroy()
        {
            OnLinkClicked = null;
        }
    }
}