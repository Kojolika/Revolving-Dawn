using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Serialization;
using Tooling.Logging;
using Tooling.StaticData.Validation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Directory = System.IO.Directory;

namespace Tooling.StaticData
{
    /// <summary>
    /// Designed to be a CMS system where we manage our static data.
    /// </summary>
    public class EditorWindow : UnityEditor.EditorWindow
    {
        private const string EditorWindowName = "Static Data Editor";

        [MenuItem("KoJy/Open Static Data Editor")]
        private static void OpenStaticDataEditor()
        {
            GetWindow<EditorWindow>(EditorWindowName);
        }

        /// <summary>
        /// Stores the index of the static data type that the user was looking at before hot reloading.
        /// </summary>
        private int selectedIndex;

        /// <summary>
        /// Stores the type of the static data instance that the user was looking at before hot reloading.
        /// </summary>
        private Type selectedType;

        /// <summary>
        /// <see cref="ListView"/> wrapper of all our of different static data types.
        /// </summary>
        private TypesView typesListView;

        /// <summary>
        /// <see cref="ListView"/> of static data instances of the currently selected static data type.
        /// </summary>
        private InstancesView instancesView;

        /// <summary>
        /// <see cref="BindingFlags"/> used to find fields on our static data instances.
        /// </summary>
        public const BindingFlags BindingFlagsToSelectStaticDataFields = BindingFlags.Instance | BindingFlags.Public;

        public static readonly StyleColor BorderColor = new(Color.gray);
        public static readonly StyleFloat BorderWidth = new(0.5f);
        private const float ToolbarButtonWidth = 180;

        private static readonly string StaticDataDirectory = Path.Join(Application.dataPath, "StaticData");

        public void CreateGUI()
        {
            StaticDatabase.Instance.BuildDictionaryFromJson();

            var root = rootVisualElement;

            var topToolBar = CreateTopToolBar();
            root.Add(topToolBar);

            typesListView = new TypesView();
            typesListView.ListView.selectionChanged += _ => selectedIndex = typesListView.ListView.selectedIndex;

            var rightPanel = new VisualElement();
            rightPanel.Add(CreateRightPanel(typesListView.ListView));

            var twoPanelSplit = new TwoPaneSplitView
            {
                fixedPaneInitialDimension = 140,
                orientation = TwoPaneSplitViewOrientation.Horizontal
            };
            twoPanelSplit.Add(typesListView);
            twoPanelSplit.Add(rightPanel);
            root.Add(twoPanelSplit);
        }

        private VisualElement CreateTopToolBar()
        {
            var root = new VisualElement
            {
                style =
                {
                    borderBottomColor = BorderColor,
                    borderBottomWidth = BorderWidth,
                    paddingBottom = 8f,
                    flexDirection = FlexDirection.Row
                }
            };

            root.Add(CreateSaveAndExportJsonButton());
            root.Add(CreateSaveAndExportDatabaseButton());
            root.Add(CreateValidateButton());
            root.Add(WipeJsonButton());

            return root;
        }

        private VisualElement CreateSaveAndExportJsonButton()
        {
            var root = new VisualElement();

            var button = new ToolbarButton(() => { _ = SaveAllStaticDataToJson(true); })
            {
                text = "Validate then Save to Json",
                style = { width = ToolbarButtonWidth }
            };

            root.Add(button);

            return root;
        }

        private async UniTask SaveAllStaticDataToJson(bool shouldValidateData)
        {
            if (shouldValidateData)
            {
                ValidateStaticData();
            }

            await StaticDatabase.Instance.SaveAllStaticDataToJson();
        }

        private VisualElement CreateSaveAndExportDatabaseButton()
        {
            var root = new VisualElement();

            var button = new ToolbarButton(() => { MyLogger.Log("Saving and exporting to database..."); })
            {
                text = "Save and Export to Database",
                style = { width = ToolbarButtonWidth }
            };

            root.Add(button);

            return root;
        }

        private VisualElement CreateValidateButton()
        {
            var root = new VisualElement();

            var button = new ToolbarButton(ValidateStaticData)
            {
                text = "Validate Data",
                style = { width = ToolbarButtonWidth }
            };

            root.Add(button);

            return root;
        }

        private void ValidateStaticData()
        {
            StaticDatabase.Instance.ValidateStaticData();

            // can be null if a type hasn't been selected yet (like when the menu is first opened)
            instancesView?.ListView?.RefreshItems();
            typesListView.ListView.RefreshItems();
        }

        private VisualElement WipeJsonButton()
        {
            var root = new VisualElement();

            var button = new ToolbarButton(RemoveAllJson)
            {
                text = "Remove All Json",
                style = { width = ToolbarButtonWidth }
            };

            root.Add(button);

            return root;
        }

        private void RemoveAllJson()
        {
            var staticDataDirectory = new DirectoryInfo(StaticDataDirectory);
            foreach (var directory in staticDataDirectory.GetDirectories())
            {
                directory.Delete(true);
            }

            StaticDatabase.Instance.Clear();

            // can be null if a type hasn't been selected yet (like when the menu is first opened)
            instancesView?.ListView?.Rebuild();
            typesListView.ListView.Rebuild();
        }

        /// <param name="staticDataTypesListView">The list of static data types that the right panel reacts to.</param>
        private VisualElement CreateRightPanel(ListView staticDataTypesListView)
        {
            var twoPaneSplitView = new TwoPaneSplitView
            {
                fixedPaneIndex = 0,
                fixedPaneInitialDimension = 100,
                orientation = TwoPaneSplitViewOrientation.Vertical
            };

            // temp until we populate a type
            twoPaneSplitView.Add(new Label("Select a static data type."));
            twoPaneSplitView.Add(new VisualElement());

            staticDataTypesListView.selectionChanged += _ =>
            {
                selectedType = StaticDatabase.Instance.GetAllStaticDataTypes()[selectedIndex];

                twoPaneSplitView.Clear();

                if (selectedType == null)
                {
                    return;
                }
            
                // TODO: figure out why split view is being weird
                var validatorErrorView = new ValidatorErrorView(selectedType);
                instancesView = new InstancesView(selectedType, true, validatorErrorView.OnStaticDataSelected);

                twoPaneSplitView.Add(instancesView);
                twoPaneSplitView.Add(validatorErrorView);
            };

            return twoPaneSplitView;
        }

        public class InstanceEditorWindow : UnityEditor.EditorWindow
        {
            private Type selectedType;
            private StaticData editingObj;
            private EditorWindow openedEditorWindow;
            private bool isInitialized;

            public void Initialize(StaticData editingObj, Type selectedType)
            {
                rootVisualElement.Clear();
                this.selectedType = selectedType;
                this.editingObj = editingObj;
                openedEditorWindow = GetWindow<EditorWindow>();

                isInitialized = true;

                CreateGUI();
                Show();
                Focus();
            }

            public void CreateGUI()
            {
                if (!isInitialized)
                {
                    return;
                }

                var root = rootVisualElement;

                saveChangesMessage = "You have unsaved changes. Do you want to save these changes?";

                titleContent = new GUIContent($"New {selectedType?.Name}");
                root.Add(new Button(() =>
                {
                    SaveChanges();
                    Close();
                })
                {
                    text = "Save and close",
                    style = { minWidth = 200 }
                });
                foreach (var field in selectedType!.GetFields(BindingFlagsToSelectStaticDataFields))
                {
                    var row = new VisualElement
                    {
                        style = { flexDirection = FlexDirection.Row }
                    };
                    row.Add(new Label($"{field.Name}")
                    {
                        style = { minWidth = 100 }
                    });
                    row.Add(new GeneralField(field, editingObj, _ => { hasUnsavedChanges = true; }));
                    root.Add(row);
                }
            }

            public override void SaveChanges()
            {
                StaticDatabase.Instance.Add(editingObj);
                openedEditorWindow.instancesView.Refresh();
                base.SaveChanges();
            }
        }
    }
}