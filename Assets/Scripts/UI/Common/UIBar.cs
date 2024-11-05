using System;
using Tooling.Logging;
using UnityEngine;

namespace UI.Common
{
    [RequireComponent(typeof(RectTransform))]
    public class UIBar : MonoBehaviour
    {
        [SerializeField] private float maxValue;
        [SerializeField] private float minValue;
        [SerializeField] private float currentValue;
        [SerializeField] private UIBarFill fill;

        private void Awake()
        {
            if (fill == null)
            {
                MyLogger.LogError("Bar fill is null!");
            }
        }

        public void Initialize(float minValue, float maxValue, float currentValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.currentValue = currentValue;
        }

        public void SetFillValue(float value)
        {
            currentValue = value;
        }
    }
}