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
            GetWindow<EditorWindow>(EditorWindowName).Open();
        }

        /// <summary>
        /// Stores the index of the static data type that the user was looking at before hot reloading.
        /// </summary>
        private int selectedIndex;

        /// <summary>
        /// Stores the type of the static data instance that the user was looking at before hot reloading.
        /// </summary>
        private System.Type selectedType;

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

        private ThemeStyleSheet defaultTheme;

        private static readonly string StaticDataDirectory = Path.Join(Application.dataPath, "StaticData");

        private bool isInitialized;

        private void Open()
        {
            defaultTheme = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>("Assets/UIToolkit/EditorTheme.tss");
            if (defaultTheme == null)
            {
                MyLogger.LogError("Could not load theme style sheet!");
            }

            rootVisualElement.styleSheets.Add(defaultTheme);

            isInitialized = true;
            CreateGUI();
        }

        public void CreateGUI()
        {
            if (!isInitialized)
            {
                Open();
                return;
            }

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
            var root = new VisualElement();
            root.AddToClassList(VisualElementClasses.ToolbarName);
            root.AddToClassList(VisualElementClasses.BorderBottom);

            root.Add(CreateToolbarButton("Validate then Save to Json", () => _ = SaveAllStaticDataToJson(true)));
            root.Add(CreateToolbarButton("Save and Export to Database", () => MyLogger.Log("Saving and exporting to database...")));
            root.Add(CreateToolbarButton("Validate Data", StaticDatabase.Instance.ValidateStaticData));
            root.Add(CreateToolbarButton("Remove All Json", RemoveAllJson));

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

        private ToolbarButton CreateToolbarButton(string text, Action onClick = null)
        {
            var button = new ToolbarButton(() => onClick?.Invoke())
            {
                text = text,
            };
            button.AddToClassList(VisualElementClasses.ToolBarButtonName);
            button.AddToClassList(VisualElementClasses.NoBorder);
            return button;
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

            public Context staticDataContext { get; private set; }

            public static void Open(StaticData editingObj, Type selectedType)
            {
                GetWindow<InstanceEditorWindow>().Initialize(editingObj, selectedType);
            }

            private void Initialize(StaticData editingObj, Type selectedType)
            {
                rootVisualElement.Clear();
                this.selectedType = selectedType;
                this.editingObj = editingObj;

                staticDataContext = new Context(editingObj);

                openedEditorWindow = GetWindow<EditorWindow>();
                rootVisualElement.styleSheets.Add(openedEditorWindow.defaultTheme);
                rootVisualElement.AddToClassList(VisualElementClasses.PaddingLarge);
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

                if (DrawerManager.Instance.StaticDataDrawers.TryGetValue(selectedType, out var customDrawer))
                {
                    customDrawer.OnValueChanged += () => hasUnsavedChanges = true;
                    root.Add(customDrawer.Draw(editingObj));
                }
                else
                {
                    var generalField = new GeneralField(
                        selectedType,
                        new ValueProvider<StaticData>(() => editingObj, staticData => editingObj = staticData, editingObj?.Name),
                        new GeneralField.Options { EnumerateStaticDataProperties = true });
                    generalField.OnValueChanged += _ => hasUnsavedChanges = true;
                    root.Add(generalField);
                }

                var saveButton = new Button(() =>
                {
                    SaveChanges();
                    Close();
                })
                {
                    text = "Save and close",
                };
                saveButton.AddToClassList(VisualElementClasses.InstanceSaveButton);

                root.Add(saveButton);
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

            public override void DiscardChanges()
            {
                base.DiscardChanges();
            }
        }
    }
}