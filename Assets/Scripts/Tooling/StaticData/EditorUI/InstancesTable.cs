using System;
using System.Collections.Generic;
using System.Linq;
using Tooling.Logging;
using UnityEngine.UIElements;
using Utils.Extensions;

namespace Tooling.StaticData.EditorUI.EditorUI
{
    /// <summary>
    /// Displays a list of static data instances as rows in a table
    /// </summary>
    public class InstancesTable : VisualElement
    {
        private ListView listView;

        private readonly bool               allowEditing;
        private readonly Action<StaticData> onSelectionChanged;

        public readonly Type SelectedType;

        private const float RowPadding = 4f;

        private Dictionary<Type, Dictionary<StaticData, List<string>>> validationErrors;

        public InstancesTable(Type selectedType, bool allowEditing, Action<StaticData> onSelectionChanged)
        {
            this.SelectedType       = selectedType;
            this.allowEditing       = allowEditing;
            this.onSelectionChanged = onSelectionChanged;

            StaticDatabase.Instance.InstancesUpdated    += OnInstancesUpdated;
            StaticDatabase.Instance.ValidationCompleted += OnValidationCompleted;

            RefreshView();
        }

        ~InstancesTable()
        {
            StaticDatabase.Instance.InstancesUpdated    -= OnInstancesUpdated;
            StaticDatabase.Instance.ValidationCompleted -= OnValidationCompleted;
        }

        private void RefreshView()
        {
            Clear();

            var instances = StaticDatabase.Instance.GetInstancesForType(SelectedType);
            if (!allowEditing)
            {
                // allow selection of null
                instances.Insert(0, null);
            }

            validationErrors = StaticDatabase.Instance.validationErrors;

            listView = new ListView
            {
                makeItem = () => new InstanceRow(SelectedType, allowEditing),
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
                    (item as InstanceRow)!.BindItem(instance);
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
                    instances[index] = Activator.CreateInstance(SelectedType) as StaticData;
                    string newInstanceName = StaticDatabase.Instance.GetStaticDataInstance(SelectedType, $"{SelectedType.Name}_{index}") == null
                        ? $"{SelectedType.Name}_{index}"
                        : $"{SelectedType.Name}_{index}(1)";

                    instances[index].Name = newInstanceName;
                }

                StaticDatabase.Instance.UpdateInstancesForType(SelectedType, instances);
            };

            // listview already removes the element, just update our StaticData dict
            listView.itemsRemoved     += _ => StaticDatabase.Instance.UpdateInstancesForType(SelectedType, instances);
            listView.selectionChanged += selectedObjects => onSelectionChanged?.Invoke(selectedObjects.FirstOrDefault() as StaticData);

            Add(CreateInstanceHeader(SelectedType));
            Add(listView);
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

        private void OnValidationCompleted()
        {
            validationErrors = StaticDatabase.Instance.validationErrors;
            listView.RefreshItems();
        }

        private void OnInstancesUpdated(Type type)
        {
            if (type != SelectedType)
            {
                return;
            }

            RefreshView();
        }

        /// <summary>
        /// Allows selecting a static data type, similar to how we select objects in unity.
        /// </summary>
        public class Selector : UnityEditor.EditorWindow
        {
            private bool               isInitialized;
            private Type               staticDataType;
            private Action<StaticData> onSelectionChanged;

            public static Selector Open(Type staticDataType, Action<StaticData> onSelectionChanged)
            {
                var instanceSelector = GetWindow<Selector>();
                instanceSelector.staticDataType     = staticDataType;
                instanceSelector.onSelectionChanged = onSelectionChanged;

                instanceSelector.Initialize();
                instanceSelector.Show(true);
                instanceSelector.Focus();

                return instanceSelector;
            }

            private void Initialize()
            {
                isInitialized = true;
                CreateGUI();
            }

            public void CreateGUI()
            {
                rootVisualElement.Clear();
                if (!isInitialized)
                {
                    return;
                }

                // Append closing this window to the on select action 
                onSelectionChanged += _ =>
                {
                    Close();
                    var instanceEditorWindow = GetWindow<EditorWindow.InstanceEditorWindow>();
                    instanceEditorWindow.Show();
                    instanceEditorWindow.Focus();
                };

                var instancesView = new InstancesTable(staticDataType, false, onSelectionChanged);
                rootVisualElement.Add(instancesView);
            }
        }
    }
}