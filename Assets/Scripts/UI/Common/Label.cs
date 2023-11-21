using TMPro;
using UnityEngine;

namespace UI.Common
{
    public interface ILabel
    {
        void SetText(string text);
    }
    public class Label : MonoBehaviour, ILabel
    {
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        public void SetText(string text)
        {
            textMeshProUGUI.text = text;
        }
    }
}