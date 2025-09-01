using Tooling.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(ContentSizeFitter))]
    public class UIBarFill : MonoBehaviour
    {
        private enum Direction
        {
            Horizontal,
            Vertical
        }

        [SerializeField] private UIBar parentUIBar;
        [SerializeField] private Direction barDirection;
        
        private void Awake()
        {
            if (parentUIBar == null)
            {
                MyLogger.Error("Parent UI Bar fill is null!");
            }
        }
    }
}