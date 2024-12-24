using System;
using System.Collections.Generic;
using Tooling.Logging;
using UnityEngine.UIElements;

namespace Tooling.StaticData
{
    public class TypesView : VisualElement
    {
        public readonly ListView ListView;
        
        private EditorWindow editorWindow => UnityEditor.EditorWindow.GetWindow<EditorWindow>();
        private List<Type> staticDataTypes => StaticDatabase.Instance.GetAllStaticDataTypes();
        private Dictionary<Type, Dictionary<StaticData, List<string>>> validationErrors => StaticDatabase.Instance.validationErrors;

        public TypesView()
        {
            ListView = new ListView
            {
                makeItem = () => new TypeView(),
                bindItem = (item, index) =>
                {
                    int numValidationErrors = validationErrors?.TryGetValue(staticDataTypes[index], out var instanceValidationDict) ?? false
                        ? instanceValidationDict.Count
                        : 0;

                    ((TypeView)item).BindItem(staticDataTypes[index], numValidationErrors);
                },
                unbindItem = (item, _) => ((TypeView)item).UnBindItem(),
                itemsSource = staticDataTypes,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All
            };

            Add(ListView);
        }
    }
}