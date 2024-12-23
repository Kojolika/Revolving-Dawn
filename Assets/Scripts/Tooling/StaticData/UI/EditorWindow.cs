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
        /// The list of types in our project that derive from <see cref="StaticData"/>.
        /// </summary>
        public List<Type> staticDataTypes { get; private set; }

        /// <summary>
        /// Dictionary which maps a derived <see cref="StaticData"/> type to saved instances of that type in json.
        /// </summary>
        public Dictionary<Type, List<StaticData>> staticDataInstances { get; private set; }

        /// <summary>
        /// Dictionary that maps a static data type to instances and their current validation errors.
        /// </summary>
        public Dictionary<Type, Dictionary<StaticData, List<string>>> validationErrors { get; private set; }

        /// <summary>
        /// Stores the index of the static data type that the user was looking at before hot reloading.
        /// </summary>
        private int selectedIndex;

        /// <summary>
        /// Stores the index of the static data instance that the user was looking at before hot reloading.
        /// </summary>
        private int typeInstanceSelectedIndex;

        /// <summary>
        /// Stores the type of the static data instance that the user was looking at before hot reloading.
        /// </summary>
        public Type SelectedType { get; private set; }

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

        private JsonSerializer jsonSerializer;
        private static readonly string StaticDataDirectory = Path.Join(Application.dataPath, "StaticData");

        public void CreateGUI()
        {
            jsonSerializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                Converters = { new AssetReferenceConverter(), new ColorConverter(), /*new StaticDataConverter()*/ },
                ContractResolver = new CustomContractResolver()
            };

            var root = rootVisualElement;

            var topToolBar = CreateTopToolBar();
            topToolBar.Add(CreateSaveAndExportJsonButton());
            topToolBar.Add(CreateSaveAndExportDatabaseButton());
            topToolBar.Add(CreateValidateButton());
            root.Add(topToolBar);

            staticDataTypes = typeof(StaticData).Assembly.GetTypes()
                .Where(type => typeof(StaticData).IsAssignableFrom(type) && !type.IsAbstract)
                .ToList();
            CreateStaticDataTypeInstanceDictionary(staticDataTypes);

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

        private void CreateStaticDataTypeInstanceDictionary(List<Type> staticDataTypes)
        {
            staticDataInstances ??= new Dictionary<Type, List<StaticData>>();

            // If we hot reload while the window is open and a new static data type is created, add to our dict
            foreach (var type in staticDataTypes)
            {
                if (staticDataInstances.ContainsKey(type))
                {
                    continue;
                }

                var typeInstances = new List<StaticData>();
                var typeDirectory = Path.GetFullPath(Path.Join(StaticDataDirectory, type.Name));

                if (Directory.Exists(typeDirectory))
                {
                    foreach (var file in Directory.EnumerateFiles(typeDirectory, "*.json"))
                    {
                        using var streamReader = File.OpenText(file);

                        var staticDataFromJson = (StaticData)jsonSerializer.Deserialize(streamReader, type);
                        if (staticDataFromJson == null)
                        {
                            MyLogger.LogError($"Static Data of type {type} could not be deserialized.");
                            continue;
                        }

                        var fileNameWithExtension = new FileInfo(file).Name;
                        staticDataFromJson.Name = fileNameWithExtension[..^".json".Length];

                        typeInstances.Add(staticDataFromJson);
                    }
                }

                staticDataInstances.Add(type, typeInstances);
            }
        }

        private async UniTask SaveAllStaticDataToJson(bool shouldValidateData)
        {
            if (shouldValidateData)
            {
                ValidateStaticData();
            }

            var writeTasks = new List<UniTask>();
            foreach (var kvp in staticDataInstances)
            {
                var staticDataFilePath = Path.GetFullPath(Path.Join(StaticDataDirectory, kvp.Key.Name));
                var directoryInfo = Directory.CreateDirectory(staticDataFilePath);

                foreach (var directory in directoryInfo.GetDirectories())
                {
                    directory.Delete();
                }

                foreach (var instance in kvp.Value)
                {
                    writeTasks.Add(SaveInstanceToJson(instance, staticDataFilePath));
                }
            }

            EditorUtility.DisplayProgressBar("Saving to Json", "Saving...", 0.5f);

            try
            {
                await UniTask.WhenAll(writeTasks);
            }
            catch (Exception e)
            {
                MyLogger.LogError(e.Message);
            }

            EditorUtility.ClearProgressBar();

            return;

            // Local function
            async UniTask SaveInstanceToJson(StaticData instance, string typeDirectory)
            {
                if (!Directory.Exists(typeDirectory))
                {
                    MyLogger.LogError($"Type directory {typeDirectory} does not exist!");
                    return;
                }

                var filePath = $"{Path.Join(typeDirectory, instance.Name)}.json";
                MyLogger.Log($"Writing {instance.Name} to file: {filePath}");
                await using StreamWriter file = new StreamWriter(filePath);
                using JsonWriter writer = new JsonTextWriter(file);

                jsonSerializer.Serialize(writer, instance);
            }
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

            return root;
        }

        private VisualElement CreateSaveAndExportJsonButton()
        {
            var root = new VisualElement();

            var button = new ToolbarButton(() => { _ = SaveAllStaticDataToJson(true); })
            {
                text = "Validate then Save to Json",
                style = { width = 200 }
            };

            root.Add(button);

            return root;
        }

        private VisualElement CreateSaveAndExportDatabaseButton()
        {
            var root = new VisualElement();

            var button = new ToolbarButton(() => { MyLogger.Log("Saving and exporting to database..."); })
            {
                text = "Save and Export to Database",
                style = { width = 200 }
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
                style = { width = 200 }
            };

            root.Add(button);

            return root;
        }

        private void ValidateStaticData()
        {
            validationErrors = Validator.ValidateObjects(
                staticDataInstances.SelectMany(kvp => kvp.Value).ToList(),
                BindingFlagsToSelectStaticDataFields
            );

            // can be null if a type hasn't been selected yet (like when the menu is first opened)
            instancesView?.ListView?.RefreshItems();
            typesListView.ListView.RefreshItems();
        }

        /// <param name="staticDataTypesListView">The list of static data types that the right panel reacts to.</param>
        private VisualElement CreateRightPanel(ListView staticDataTypesListView)
        {
            var root = new VisualElement();
            var label = new Label("Select a static data type.");

            staticDataTypesListView.selectionChanged += _ =>
            {
                SelectedType = staticDataTypes?[selectedIndex];

                root.Clear();

                if (SelectedType == null)
                {
                    return;
                }

                instancesView = new InstancesView(SelectedType, true,
                    _ => { typeInstanceSelectedIndex = instancesView.ListView.selectedIndex; });

                root.Add(instancesView);
            };

            root.Add(label);

            return root;
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
                openedEditorWindow.instancesView.ListView.RefreshItems();
                base.SaveChanges();
            }
        }
    }
}