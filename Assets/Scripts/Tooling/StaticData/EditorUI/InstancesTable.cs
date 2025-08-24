using System;
using System.Collections.Generic;
using System.Linq;
using Tooling.Logging;
using UnityEngine.UIElements;
using Utils.Extensions;

namespace Tooling.StaticData.EditorUI
{
    /// <summary>
    /// Displays a list of static data instances as rows in a table
    /// </summary>
    public class InstancesTable : VisualElement
    {
        private readonly ListView listView;
        private readonly Type     selectedType;
        private readonly bool     allowEditing;

        private const float RowPadding = 4f;

        private Dictionary<Type, Dictionary<StaticData, List<string>>> validationErrors;

        public InstancesTable(Type selectedType, bool allowEditing, Action<StaticData> onSelectionChanged)
        {
            this.selectedType = selectedType;
            this.allowEditing = allowEditing;

            var instances = StaticDatabase.Instance.GetInstancesForType(selectedType);
            if (!allowEditing)
            {
                // allow selection of null
                instances.Insert(0, null);
            }

            validationErrors = StaticDatabase.Instance.validationErrors;

            listView = new ListView
            {
                makeItem = () => new InstanceRow(selectedType, allowEditing),
                bindItem = (item, index) =>
                {
                    if (instances.IsNullOrEmpty()
                     || instances == null
                     || index < 0
                     || index >= instances.Count)
                    {
                        return;
                    }

                    var instance = instances[index];
                    (item as InstanceRow)!.BindItem(instance,
                                                     instance != null && (validationErrors?.TryGetValue(selectedType, out var instanceValidationDict) ?? false)
                                                         ? instanceValidationDict.GetValueOrDefault(instance)
                                                         : null
                    );
                },
                unbindItem                    = (item, _) => (item as InstanceRow)!.UnBindItem(),
                itemsSource                   = instances,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBorder                    = true,
                selectionType                 = SelectionType.Multiple,
                showAddRemoveFooter           = allowEditing,
                virtualizationMethod          = CollectionVirtualizationMethod.DynamicHeight
            };

            listView.itemsAdded += ints =>
            {
                foreach (var index in ints)
                {
                    // Item is added as null, create a new instance of that type and set the name to a unique name
                    instances[index] = Activator.CreateInstance(selectedType) as StaticData;
                    string newInstanceName = StaticDatabase.Instance.GetStaticDataInstance(selectedType, $"{selectedType.Name}_{index}") == null
                        ? $"{selectedType.Name}_{index}"
                        : $"{selectedType.Name}_{index}(1)";

                    instances[index].Name = newInstanceName;
                }

                StaticDatabase.Instance.UpdateInstancesForType(selectedType, instances);
            };

            // listview already removes the element, just update our StaticData dict
            listView.itemsRemoved += _ => StaticDatabase.Instance.UpdateInstancesForType(selectedType, instances);

            listView.selectionChanged += selectedObjects => onSelectionChanged?.Invoke(selectedObjects.FirstOrDefault() as StaticData);

            StaticDatabase.Instance.OnValidationCompleted += OnValidationCompleted;

            Add(CreateInstanceHeader(selectedType));
            Add(listView);
        }

        ~InstancesTable()
        {
            StaticDatabase.Instance.OnValidationCompleted -= OnValidationCompleted;
        }

        public static Label CreateInstanceColumn(string labelText) => new(labelText)
        {
            style =
            {
                paddingLeft  = RowPadding,
                paddingRight = RowPadding,
                maxWidth     = 200,
                minWidth     = 200,
                overflow     = Overflow.Hidden,
                alignSelf    = Align.Center
            }
        };

        private VisualElement CreateInstanceHeader(Type staticDataType)
        {
            var header = new VisualElement
            {
                style =
                {
                    flexDirection     = FlexDirection.Row,
                    borderBottomColor = EditorWindow.BorderColor,
                    borderBottomWidth = EditorWindow.BorderWidth,
                }
            };

            if (allowEditing)
            {
                // So the column headers line up with the instance data
                header.Add(new VisualElement
                {
                    style =
                    {
                        minWidth = InstanceRow.EditButtonWidth,
                        // match margins and padding with the Button element
                        marginTop     = 1,
                        marginBottom  = 1,
                        marginLeft    = 3,
                        marginRight   = 3,
                        paddingTop    = 1,
                        paddingBottom = 1,
                        paddingRight  = 1,
                        paddingLeft   = 1
                    }
                });
            }

            foreach (var field in InstanceRow.GetOrderedFields(staticDataType))
            {
                header.Add(CreateInstanceColumn($"{field.Name}"));
            }

            return header;
        }

        public void Refresh()
        {
            listView.itemsSource = StaticDatabase.Instance.GetInstancesForType(selectedType);
            listView.Rebuild();
        }

        private void OnValidationCompleted()
        {
            validationErrors = StaticDatabase.Instance.validationErrors;
            listView.RefreshItems();
        }

        /// <summary>
        /// Allows selecting a static data type, similar to how we select objects in unity.
        /// </summary>
        public class Selector : UnityEditor.EditorWindow
        {
            private bool                    isInitialized;
            private Type                    staticDataType;
            public event Action<StaticData> onSelectionChanged;

            public static Selector Open(Type staticDataType)
            {
                var instanceSelector = GetWindow<Selector>();
                instanceSelector.Initialize(staticDataType);
                instanceSelector.Show(true);
                instanceSelector.Focus();

                return instanceSelector;
            }

            private void Initialize(Type staticDataType)
            {
                this.staticDataType = staticDataType;
                isInitialized       = true;

                CreateGUI();
            }

            public void CreateGUI()
            {
                rootVisualElement.Clear();
                if (!isInitialized)
                {
                    return;
                }

                var instancesView = new InstancesTable(staticDataType, false, onSelectionChanged);
                instancesView.listView.selectionChanged += _ =>
                {
                    Close();
                    var instanceEditorWindow = GetWindow<EditorWindow.InstanceEditorWindow>();
                    instanceEditorWindow.Show();
                    instanceEditorWindow.Focus();
                };
                rootVisualElement.Add(instancesView);
            }
        }
    }
}