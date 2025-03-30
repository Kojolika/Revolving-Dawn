using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Tooling.StaticData
{
    public class TypesView : VisualElement
    {
        public readonly ListView ListView;
        private List<System.Type> staticDataTypes => StaticDatabase.Instance.GetAllStaticDataTypes();
        private Dictionary<System.Type, Dictionary<StaticData, List<string>>> validationErrors;

        public TypesView()
        {
            ListView = new ListView
            {
                makeItem = () => new TypeView(),
                bindItem = (item, index) =>
                {
                    if (index < 0 || index >= staticDataTypes.Count)
                    {
                        return;
                    }

                    int numValidationErrors = validationErrors?.TryGetValue(staticDataTypes[index], out var instanceValidationDict) ?? false
                        ? instanceValidationDict.Count
                        : 0;

                    ((TypeView)item).BindItem(staticDataTypes[index], numValidationErrors);
                },
                unbindItem = (item, _) => ((TypeView)item).UnBindItem(),
                itemsSource = staticDataTypes,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All
            };

            StaticDatabase.Instance.OnValidationCompleted += OnValidationCompleted;

            validationErrors = StaticDatabase.Instance.validationErrors;

            Add(ListView);
        }

        ~TypesView()
        {
            StaticDatabase.Instance.OnValidationCompleted -= OnValidationCompleted;
        }

        private void OnValidationCompleted()
        {
            validationErrors = StaticDatabase.Instance.validationErrors;
            ListView.RefreshItems();
        }
    }
}