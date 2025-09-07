#if !PRODUCTION || ENABLE_DEBUG_MENU
using System;
using System.Collections.Generic;
using TMPro;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace Koj.Debug
{
    public abstract class Page : MonoBehaviour
    {
        // TODO: cache value color too
        private readonly List<(TextMeshProUGUI component, Func<string> valueGetter)> updateableLabels = new();

        protected void AddLabel(string label)
        {
            var labelGo = new GameObject();
            labelGo.transform.SetParent(transform);
            var contentSizeFitter = labelGo.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            var labelTMP = labelGo.AddComponent<TextMeshProUGUI>();
            labelTMP.autoSizeTextContainer = true;
            labelTMP.enableAutoSizing      = true;
            labelTMP.text                  = $"{label}";
        }

        /// <summary>
        /// Instantiates a label gameobject with text to the page
        /// </summary>
        /// <param name="label">The label itself</param>
        /// <param name="valueGetter"> Function that returns the value of the label,
        /// we use a function so we can update it in OnEnable</param>
        /// <param name="valueColor"> Color to display the value </param>
        protected void AddLabelWithValue(string label, Func<string> valueGetter, Color valueColor = default)
        {
            var labelParent = new GameObject();
            labelParent.transform.SetParent(transform);
            var horizontalLayoutGroup = labelParent.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childAlignment    = TextAnchor.MiddleCenter;
            horizontalLayoutGroup.childControlWidth = true;
            horizontalLayoutGroup.spacing           = 20f;
            var contentSizeFitter = labelParent.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            var labelGo = new GameObject();
            labelGo.transform.SetParent(labelParent.transform);
            contentSizeFitter               = labelGo.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            var labelTMP = labelGo.AddComponent<TextMeshProUGUI>();
            labelTMP.autoSizeTextContainer = true;
            labelTMP.enableAutoSizing      = true;
            labelTMP.text                  = $"{label}:";

            var valueGo = new GameObject();
            valueGo.transform.SetParent(labelParent.transform);
            contentSizeFitter               = valueGo.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            var valueTMP = valueGo.AddComponent<TextMeshProUGUI>();
            valueTMP.autoSizeTextContainer = true;
            valueTMP.enableAutoSizing      = true;
            valueTMP.text                  = $"<color=#{ColorUtility.ToHtmlStringRGB(valueColor)}>{valueGetter.Invoke()}</color>";

            updateableLabels.Add((valueTMP, valueGetter));
        }

        private void OnEnable()
        {
            foreach (var label in updateableLabels)
            {
                MyLogger.Info($"On enable, setting {label.component} to {label.valueGetter.Invoke()}");
                label.component.text = label.valueGetter.Invoke();
            }
        }
    }
}
#endif