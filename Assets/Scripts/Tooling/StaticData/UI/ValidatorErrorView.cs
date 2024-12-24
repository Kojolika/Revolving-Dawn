using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tooling.StaticData
{
    public class ValidatorErrorView : VisualElement
    {
        private readonly Type selectedType;
        private StaticData instance;

        public ValidatorErrorView(Type selectedType)
        {
            this.selectedType = selectedType;

            OnValidate();
        }

        public void OnStaticDataSelected(StaticData staticData)
        {
            instance = staticData;

            OnValidate();
        }

        // TODO: add a callback in StaticDatabase when validation results are updated
        private void OnValidate()
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
                    style = { color = Color.red }
                });
            }
        }
    }
}