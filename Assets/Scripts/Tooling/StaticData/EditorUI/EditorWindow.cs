using System;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Tooling.Logging;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
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

        public static readonly StyleColor BorderColor = new(Color.gray);
        public static readonly StyleFloat BorderWidth = new(0.5f);
        private const float ToolbarButtonWidth = 180;

        private static readonly string StaticDataDirectory = Path.Join(Application.dataPath, "StaticData");

        public void CreateGUI()
        {
            var root = rootVisualElement;
            root.Clear();

            StaticDatabase.Instance.BuildDictionaryFromJson();

            var topToolBar = CreateTopToolBar();
            root.Add(topToolBar);

            typesListView = new TypesView();

            var rightPanel = CreateRightPanel();
            typesListView.ListView.selectionChanged += _ =>
            {
                selectedIndex = typesListView.ListView.selectedIndex;
                selectedType = StaticDatabase.Instance.GetAllStaticDataTypes()[selectedIndex];

                rightPanel.Clear();

                if (selectedType == null)
                {
                    return;
                }

                // TODO: figure out why split view is being weird
                var validatorErrorView = new ValidatorErrorView(selectedType);
                instancesView = new InstancesView(selectedType, true, validatorErrorView.OnStaticDataSelected);

                rightPanel.Add(instancesView);
                rightPanel.Add(validatorErrorView);
            };

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
                StaticDatabase.Instance.ValidateStaticData();
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

            var button = new ToolbarButton(StaticDatabase.Instance.ValidateStaticData)
            {
                text = "Validate Data",
                style = { width = ToolbarButtonWidth }
            };

            root.Add(button);

            return root;
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

            foreach (var file in staticDataDirectory.GetFiles())
            {
                file.Delete();
            }

            StaticDatabase.Instance.Clear();
            CreateGUI();
        }

        private TwoPaneSplitView CreateRightPanel()
        {
            var twoPaneSplitView = new TwoPaneSplitView
            {
                fixedPaneIndex = 1,
                fixedPaneInitialDimension = 100,
                orientation = TwoPaneSplitViewOrientation.Vertical
            };

            // temp until we populate a type
            twoPaneSplitView.Add(new Label("Select a static data type."));
            twoPaneSplitView.Add(new VisualElement());

            return twoPaneSplitView;
        }

        public class InstanceEditorWindow : UnityEditor.EditorWindow
        {
            private Type selectedType;
            private StaticData editingObj;
            private EditorWindow openedEditorWindow;
            private bool isInitialized;
            private string nameOnOpening;

            public void Initialize(StaticData editingObj, Type selectedType)
            {
                rootVisualElement.Clear();
                this.selectedType = selectedType;
                this.editingObj = editingObj;
                openedEditorWindow = GetWindow<EditorWindow>();
                nameOnOpening = editingObj.Name;

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

                titleContent = new GUIContent($"Edit {editingObj?.Name}");
                root.Add(new Button(() =>
                {
                    SaveChanges();
                    Close();
                })
                {
                    text = "Save and close",
                    style = { minWidth = 200 }
                });

                var fields = Utils.GetFields(selectedType);

                // Move name to the top of our editor
                if (fields.Count > 1)
                {
                    var nameField = fields.First(field => field.Name == nameof(StaticData.Name));
                    var nameIndex = fields.IndexOf(nameField);
                    (fields[0], fields[nameIndex]) = (fields[nameIndex], fields[0]);
                }

                foreach (var field in fields)
                {
                    var row = new VisualElement
                    {
                        style = { flexDirection = FlexDirection.Row }
                    };

                    row.Add(new Label($"{field.Name}")
                    {
                        style = { minWidth = 100 }
                    });

                    row.Add(
                        new GeneralField(
                            field.FieldType,
                            editingObj,
                            new FieldValueProvider(field),
                            callback: _ => hasUnsavedChanges = true
                        )
                    );
                    root.Add(row);
                }
            }

            public override void SaveChanges()
            {
                if (StaticDatabase.Instance.GetStaticDataInstance(selectedType, nameOnOpening) is not null)
                {
                    StaticDatabase.Instance.Remove(selectedType, nameOnOpening);
                }

                StaticDatabase.Instance.Add(editingObj);
                openedEditorWindow.instancesView.Refresh();
                base.SaveChanges();
            }
        }
    }
}