using TMPro;
using UnityEngine;

namespace UI.Common
{
    public class Label : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        public void SetText(string text)
        {
            textMeshProUGUI.text = text;
        }
    }
}