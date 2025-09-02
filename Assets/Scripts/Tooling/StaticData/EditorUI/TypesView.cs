using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Tooling.StaticData.Data
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

            StaticDatabase.Instance.ValidationCompleted += ValidationCompleted;

            validationErrors = StaticDatabase.Instance.validationErrors;

            Add(ListView);
        }

        ~TypesView()
        {
            StaticDatabase.Instance.ValidationCompleted -= ValidationCompleted;
        }

        private void ValidationCompleted()
        {
            validationErrors = StaticDatabase.Instance.validationErrors;
            ListView.RefreshItems();
        }
    }
}