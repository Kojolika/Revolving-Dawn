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
        [SerializeField] private RectTransform content;

        private readonly List<LabelData> updateableLabels = new();

        protected void AddButton(Func<string> labelGetter, Action action)
        {
            var buttonGo = new GameObject("Button");
            buttonGo.transform.SetParent(content);
            var contentSizeFitter = buttonGo.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(buttonGo.transform);
            var labelTMP = labelGo.AddComponent<TextMeshProUGUI>();
            labelTMP.autoSizeTextContainer = true;
            labelTMP.enableAutoSizing      = true;
            labelTMP.text                  = $"{labelGetter?.Invoke()}";

            var button = buttonGo.AddComponent<Button>();
            button.onClick.AddListener(() =>
            {
                action?.Invoke();
                labelTMP.text = $"{labelGetter?.Invoke()}";
            });


            updateableLabels.Add(new LabelData { Component = labelTMP, ValueGetter = labelGetter });
        }

        protected void AddLabel(string label)
        {
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(content);
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
            var labelParent = new GameObject("LabelParent");
            labelParent.transform.SetParent(content);
            var horizontalLayoutGroup = labelParent.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childAlignment    = TextAnchor.MiddleLeft;
            horizontalLayoutGroup.childControlWidth = true;
            horizontalLayoutGroup.spacing           = 20f;
            var contentSizeFitter = labelParent.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(labelParent.transform);
            contentSizeFitter               = labelGo.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            var labelTMP = labelGo.AddComponent<TextMeshProUGUI>();
            labelTMP.autoSizeTextContainer = true;
            labelTMP.enableAutoSizing      = true;
            labelTMP.text                  = $"{label}:";

            var valueGo = new GameObject("Value");
            valueGo.transform.SetParent(labelParent.transform);
            contentSizeFitter               = valueGo.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            var valueTMP = valueGo.AddComponent<TextMeshProUGUI>();
            valueTMP.autoSizeTextContainer = true;
            valueTMP.enableAutoSizing      = true;
            valueTMP.text                  = GetLabelText(valueGetter, valueColor);

            updateableLabels.Add(new LabelData
            {
                Component   = valueTMP,
                ValueGetter = valueGetter,
                ValueColor  = valueColor
            });
        }

        private static string GetLabelText(Func<string> valueGetter, Color valueColor)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(valueColor)}>{valueGetter.Invoke()}</color>";
        }

        private void OnEnable()
        {
            foreach (var label in updateableLabels)
            {
                label.Component.text = GetLabelText(label.ValueGetter, label.ValueColor);
            }
        }

        private class LabelData
        {
            public TextMeshProUGUI Component;
            public Func<string>    ValueGetter;
            public Color           ValueColor;
        }
    }
}
#endif