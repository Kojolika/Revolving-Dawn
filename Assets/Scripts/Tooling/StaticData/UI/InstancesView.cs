using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using Utils.Extensions;

namespace Tooling.StaticData
{
    public class InstancesView : VisualElement
    {
        private readonly ListView listView;
        private List<StaticData> instances;
        private readonly Type selectedType;
        private readonly bool allowEditing;

        private const float RowPadding = 4f;

        private Dictionary<Type, Dictionary<StaticData, List<string>>> validationErrors;

        public InstancesView(Type selectedType, bool allowEditing, Action<StaticData> onSelectionChanged)
        {
            this.selectedType = selectedType;
            this.allowEditing = allowEditing;

            instances = StaticDatabase.Instance.GetInstancesForType(selectedType);
            if (!allowEditing)
            {
                // allow selection of null
                instances.Insert(0, null);
            }

            validationErrors = StaticDatabase.Instance.validationErrors;

            listView = new ListView
            {
                makeItem = () => new InstanceView(selectedType, allowEditing),
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
                    (item as InstanceView)!.BindItem(instance,
                        instance != null && (validationErrors?.TryGetValue(selectedType, out var instanceValidationDict) ?? false)
                            ? instanceValidationDict.GetValueOrDefault(instance)
                            : null
                    );
                },
                unbindItem = (item, _) => (item as InstanceView)!.UnBindItem(),
                itemsSource = instances,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBorder = true,
                selectionType = SelectionType.Multiple,
                showAddRemoveFooter = allowEditing
            };

            listView.itemsAdded += ints =>
            {
                foreach (var index in ints)
                {
                    instances[index] = Activator.CreateInstance(selectedType) as StaticData;
                    instances[index].Name = $"{selectedType.Name}_{index}";
                }

                StaticDatabase.Instance.UpdateInstancesForType(selectedType, instances);
            };

            listView.itemsRemoved += ints =>
            {
                instances = instances.Where((_, index) => !ints.Contains(index)).ToList();
                StaticDatabase.Instance.UpdateInstancesForType(selectedType, instances);
            };

            listView.selectionChanged += selectedObjects => onSelectionChanged?.Invoke(selectedObjects.FirstOrDefault() as StaticData);

            StaticDatabase.Instance.OnValidationCompleted += OnValidationCompleted;

            Add(CreateInstanceHeader(selectedType));
            Add(listView);
        }

        ~InstancesView()
        {
            StaticDatabase.Instance.OnValidationCompleted -= OnValidationCompleted;
        }

        public static Label CreateInstanceColumn(string labelText) => new(labelText)
        {
            style =
            {
                paddingLeft = RowPadding,
                paddingRight = RowPadding,
                maxWidth = 200,
                minWidth = 200,
                overflow = Overflow.Hidden,
                alignSelf = Align.Center
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

            if (allowEditing)
            {
                // So the column headers line up with the instance data
                header.Add(new VisualElement
                {
                    style =
                    {
                        minWidth = InstanceView.EditButtonWidth,
                        // match margins and padding with the Button element
                        marginTop = 1,
                        marginBottom = 1,
                        marginLeft = 3,
                        marginRight = 3,
                        paddingTop = 1,
                        paddingBottom = 1,
                        paddingRight = 1,
                        paddingLeft = 1
                    }
                });
            }

            foreach (var field in InstanceView.GetOrderedFields(staticDataType))
            {
                header.Add(CreateInstanceColumn($"{field.Name}"));
            }

            return header;
        }

        public void Refresh()
        {
            instances = StaticDatabase.Instance.GetInstancesForType(selectedType);
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
            private bool isInitialized;
            private Type staticDataType;
            private Action<StaticData> onSelectionChanged;

            public static void Open(Type staticDataType, Action<StaticData> onSelectionChanged)
            {
                var instanceSelector = GetWindow<Selector>();
                instanceSelector.Initialize(staticDataType, onSelectionChanged);
                instanceSelector.Show(true);
                instanceSelector.Focus();
            }

            private void Initialize(Type staticDataType, Action<StaticData> onSelectionChanged)
            {
                this.staticDataType = staticDataType;
                this.onSelectionChanged = onSelectionChanged;
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

                var instancesView = new InstancesView(staticDataType, false, onSelectionChanged);
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