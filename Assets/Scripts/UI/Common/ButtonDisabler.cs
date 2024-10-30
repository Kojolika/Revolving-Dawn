using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RectTransform))]
    public class ButtonDisabler : MonoBehaviour
    {
        [SerializeField] private float disableTime = 0.25f;
        [SerializeField] private bool waitForEvent;
        
        private Button button;

        private void Initialize()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        private void Reset() => Initialize();

        private async void OnClick()
        {
            button.interactable = false;

            await UniTask.WaitForSeconds(disableTime);
            
            button.interactable = true;
        }

        private void OnDestroy()
        {
            
        }
    }
}