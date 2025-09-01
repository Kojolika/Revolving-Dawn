using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Tooling.Logging;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Extensions;

namespace Tooling.StaticData.EditorUI.EditorUI
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
        private Type selectedType;

        /// <summary>
        /// <see cref="ListView"/> wrapper of all our of different static data types.
        /// </summary>
        private TypesView typesListView;

        private InstancesTable   instancesTable;
        private TwoPaneSplitView rightPane;

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
                MyLogger.Error("Could not load theme style sheet!");
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

            root.Add(CreateTopToolBar());

            var twoPanelSplit = new TwoPaneSplitView
            {
                fixedPaneInitialDimension = 140,
                orientation               = TwoPaneSplitViewOrientation.Horizontal,
                style =
                {
                    flexGrow   = 1,
                    flexShrink = 0
                }
            };

            rightPane     = CreateRightPane();
            typesListView = new TypesView();
            typesListView.ListView.selectionChanged += _ =>
            {
                selectedIndex = typesListView.ListView.selectedIndex;
                selectedType  = StaticDatabase.Instance.GetAllStaticDataTypes()[selectedIndex];
                OpenInstancesTable(selectedType);
            };

            twoPanelSplit.Add(typesListView);
            twoPanelSplit.Add(rightPane);
            root.Add(twoPanelSplit);
        }

        private void OpenInstancesTable(Type selectedType)
        {
            rightPane.Clear();
            if (selectedType == null)
            {
                return;
            }

            // TODO: figure out why split view is being weird
            var validatorErrorView = new ValidatorErrorView(selectedType);
            instancesTable = new InstancesTable(selectedType, true, validatorErrorView.OnStaticDataSelected);

            rightPane.Add(instancesTable);
            rightPane.Add(validatorErrorView);
        }

        /// <summary>
        /// After code compilation this reapplies the stylesheets.
        /// </summary>
        [DidReloadScripts]
        private static void OnReloadScripts()
        {
            if (HasOpenInstances<EditorWindow>())
            {
                GetWindow<EditorWindow>().Open();
            }
        }

        private VisualElement CreateTopToolBar()
        {
            var root = new VisualElement();
            root.AddToClassList(Styles.ToolbarName);
            root.AddToClassList(Styles.BorderBottom);

            root.Add(CreateToolbarButton("Validate then Save to Json", () => _ = SaveAllStaticDataToJson(true)));
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

        private static Button CreateToolbarButton(string text, Action onClick = null)
        {
            var button = new ToolbarButton(() => onClick?.Invoke())
            {
                text = text,
            };
            button.AddToClassList(Styles.ToolBarButtonName);
            button.AddToClassList(Styles.NoBorder);
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

        private static TwoPaneSplitView CreateRightPane()
        {
            var twoPaneSplitView = new TwoPaneSplitView
            {
                fixedPaneIndex            = 1,
                fixedPaneInitialDimension = 200,
                orientation               = TwoPaneSplitViewOrientation.Vertical,
            };

            // temp until we populate a type
            twoPaneSplitView.Add(new Label("Select a static data type."));
            twoPaneSplitView.Add(new VisualElement());

            return twoPaneSplitView;
        }

        public class InstanceEditorWindow : UnityEditor.EditorWindow
        {
            private          Type         selectedType;
            private          StaticData   editingObj;
            private          EditorWindow openedEditorWindow;
            private          bool         isInitialized;
            private          string       nameOnOpening;
            private readonly List<Action> disposeActions = new();

            public static void Open(StaticData editingObj, Type selectedType)
            {
                GetWindow<InstanceEditorWindow>().Initialize(editingObj, selectedType);
            }

            private void Initialize(StaticData editingObj, Type selectedType)
            {
                rootVisualElement.Clear();
                this.selectedType = selectedType;
                this.editingObj   = editingObj;

                openedEditorWindow = GetWindow<EditorWindow>();
                rootVisualElement.styleSheets.Add(openedEditorWindow.defaultTheme);
                rootVisualElement.AddToClassList(Styles.PaddingLarge);
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
                titleContent       = new GUIContent($"Edit {editingObj?.Name}");

                if (DrawerManager.Instance.StaticDataDrawers.TryGetValue(selectedType, out var customDrawer))
                {
                    Action onValueChanged = () => hasUnsavedChanges = true;
                    customDrawer.OnValueChanged += onValueChanged;
                    AddDisposeAction(() => customDrawer.OnValueChanged -= onValueChanged);
                    root.Add(customDrawer.Draw(editingObj));
                }
                else
                {
                    var generalField = new GeneralField(
                        selectedType,
                        new ValueProvider<StaticData>(() => editingObj, staticData => editingObj = staticData, editingObj?.Name),
                        new GeneralField.Options { EnumerateStaticDataProperties = true });

                    generalField.RegisterValueChangedCallback(_ => hasUnsavedChanges = true);
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
                saveButton.AddToClassList(Styles.InstanceSaveButton);

                root.Add(saveButton);
            }

            private void AddDisposeAction(Action action)
            {
                disposeActions.Add(action);
            }

            public void OnDisable()
            {
                foreach (var action in disposeActions)
                {
                    action.Invoke();
                }
            }

            /// <summary>
            /// After code compilation this redraws the window.
            /// </summary>
            [DidReloadScripts]
            private static void OnReloadScripts()
            {
                if (!HasOpenInstances<InstanceEditorWindow>())
                {
                    return;
                }

                var instanceEditorWindow = GetWindow<InstanceEditorWindow>();
                if (instanceEditorWindow.editingObj == null)
                {
                    instanceEditorWindow.Close();
                }
                else
                {
                    instanceEditorWindow.Initialize(instanceEditorWindow.editingObj, instanceEditorWindow.selectedType);
                }
            }

            public override void SaveChanges()
            {
                if (HasNameChanged(nameOnOpening, editingObj))
                {
                    StaticDatabase.Instance.Remove(selectedType, nameOnOpening);
                }

                StaticDatabase.Instance.AddOrUpdate(editingObj);
                base.SaveChanges();
            }

            private static bool HasNameChanged(string nameOnOpening, StaticData editingObj)
            {
                return editingObj.Name != nameOnOpening;
            }
        }
    }
}