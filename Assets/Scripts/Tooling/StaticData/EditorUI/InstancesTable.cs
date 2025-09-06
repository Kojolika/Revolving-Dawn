using System;
using System.Collections.Generic;
using System.Linq;
using Tooling.Logging;
using UnityEngine.UIElements;
using Utils.Extensions;

namespace Tooling.StaticData.Data.EditorUI
{
    /// <summary>
    /// Displays a list of static data instances as rows in a table
    /// </summary>
    public class InstancesTable : VisualElement
    {
        private ListView listView;

        private readonly bool               allowEditing;
        private readonly Action<StaticData> onSelectionChanged;
        private readonly Type               selectedType;

        private const float RowPadding = 4f;

        public InstancesTable(Type selectedType, bool allowEditing, Action<StaticData> onSelectionChanged)
        {
            this.selectedType       = selectedType;
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

            var instances = StaticDatabase.Instance.GetInstancesForType(selectedType);
            if (!allowEditing)
            {
                // allow selection of null
                instances.Insert(0, null);
            }

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
                    instances[index] = Activator.CreateInstance(selectedType) as StaticData;
                    string newInstanceName = StaticDatabase.Instance.GetStaticDataInstance(selectedType, $"{selectedType.Name}_{index}") == null
                        ? $"{selectedType.Name}_{index}"
                        : $"{selectedType.Name}_{index}(1)";

                    instances[index].Name = newInstanceName;
                }

                StaticDatabase.Instance.UpdateInstancesForType(selectedType, instances);
            };

            // listview already removes the element, just update our StaticData dict
            listView.itemsRemoved     += _ => StaticDatabase.Instance.UpdateInstancesForType(selectedType, instances);
            listView.selectionChanged += selectedObjects => onSelectionChanged?.Invoke(selectedObjects.FirstOrDefault() as StaticData);

            Add(CreateInstanceHeader(selectedType));
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
            listView.RefreshItems();
        }

        private void OnInstancesUpdated(Type type)
        {
            if (type != selectedType)
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