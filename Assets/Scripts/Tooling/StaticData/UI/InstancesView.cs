using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Tooling.StaticData
{
    public class InstancesView : VisualElement
    {
        public readonly ListView ListView;

        private const float RowPadding = 4f;

        private EditorWindow editorWindow => UnityEditor.EditorWindow.GetWindow<EditorWindow>();
        private Dictionary<Type, List<StaticData>> staticDataInstances => editorWindow.staticDataInstances;
        private Dictionary<Type, Dictionary<StaticData, List<string>>> validationErrors => editorWindow.validationErrors;


        public InstancesView(Type selectedType, bool allowEditing)
        {
            ListView = new ListView
            {
                makeItem = () => new InstanceView(selectedType),
                bindItem = (item, index) =>
                {
                    var instance = staticDataInstances[selectedType][index];
                    (item as InstanceView)!.BindItem(index,
                        instance,
                        validationErrors?.TryGetValue(selectedType, out var instanceValidationDict) ?? false
                            ? instanceValidationDict.GetValueOrDefault(instance)
                            : null
                    );
                },
                unbindItem = (item, _) => (item as InstanceView)!.UnBindItem(),
                itemsSource = staticDataInstances[selectedType],
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBorder = true
            };

            Add(new Label($"{selectedType.Name} Instances"));
            if (allowEditing)
            {
                Add(new Button(() =>
                {
                    staticDataInstances[selectedType].Add(Activator.CreateInstance(selectedType) as StaticData);
                    ListView.RefreshItems();
                })
                {
                    text = $"Add new {selectedType.Name}",
                    style = { width = 200 }
                });
            }

            Add(CreateInstanceHeader(selectedType));
            Add(ListView);
        }

        public static Label CreateInstanceColumn(string labelText) => new(labelText)
        {
            style =
            {
                paddingLeft = RowPadding,
                paddingRight = RowPadding,
                maxWidth = 200,
                minWidth = 200,
            }
        };

        private VisualElement CreateInstanceHeader(Type staticDataType)
        {
            var header = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    borderBottomColor = EditorWindow.BorderColor,
                    borderBottomWidth = EditorWindow.BorderWidth,
                }
            };

            header.Add(CreateInstanceColumn("Index"));

            foreach (var field in staticDataType.GetFields(EditorWindow.BindingFlagsToSelectStaticDataFields))
            {
                header.Add(CreateInstanceColumn($"{field.Name}"));
            }

            return header;
        }
    }
}