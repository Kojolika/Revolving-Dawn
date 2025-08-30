using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tooling.StaticData
{
    public class ValidatorErrorView : VisualElement
    {
        private readonly System.Type selectedType;
        private StaticData instance;

        public ValidatorErrorView(System.Type selectedType)
        {
            this.selectedType = selectedType;

            StaticDatabase.Instance.ValidationCompleted += RefreshValidationView;
        }

        ~ValidatorErrorView()
        {
            StaticDatabase.Instance.ValidationCompleted -= RefreshValidationView;
        }

        public void OnStaticDataSelected(StaticData staticData)
        {
            instance = staticData;
            RefreshValidationView();
        }

        private void RefreshValidationView()
        {
            Clear();

            if (instance == null)
            {
                return;
            }

            if (!StaticDatabase.Instance.validationErrors.TryGetValue(selectedType, out var errorDict)
                || !errorDict.TryGetValue(instance, out var errors))
            {
                return;
            }

            foreach (var error in errors)
            {
                Add(new Label(error)
                {
                    style =
                    {
                        color = Color.red,
                        whiteSpace = WhiteSpace.Normal
                    }
                });
            }
        }
    }
}