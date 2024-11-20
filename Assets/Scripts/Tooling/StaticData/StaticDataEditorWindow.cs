using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Serialization;
using Tooling.Logging;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Directory = System.IO.Directory;

namespace Tooling.StaticData
{
    public class StaticDataEditorWindow : EditorWindow
    {
        private const string WindowName = "Static Data Editor";

        [MenuItem("KoJy/Open Static Data Editor")]
        private static void OpenStaticDataEditor()
        {
            var wnd = GetWindow<StaticDataEditorWindow>(WindowName);
            wnd.SetContext(OpenContext.Editing);
        }

        private enum OpenContext
        {
            /// <summary>
            /// When set to none, this window will not display anything.
            /// </summary>
            None,

            /// <summary>
            /// For editing static data.
            /// </summary>
            Editing,

            /// <summary>
            /// For selecting a static data type instace.
            /// </summary>
            Selection
        }

        /// <summary>
        /// Depending on what is set, changes the functionality of this editor window.
        /// <remarks>Must call <see cref="SetContext"/> after opening this window to function correctly.</remarks>
        /// </summary>
        private OpenContext openContext = OpenContext.None;

        /// <summary>
        /// The list of types in our project that derive from <see cref="StaticData"/>
        /// </summary>
        private List<Type> staticDataTypes;

        /// <summary>
        /// Dictionary which maps a derived <see cref="StaticData"/> to a dictionary of which maps the keys
        /// of that type to saved instances of that type in our database.
        /// </summary>
        private Dictionary<Type, List<StaticData>> staticDataInstances;

        /// <summary>
        /// Stores the index of the static data that the user was looking at before hot reloading.
        /// </summary>
        private int selectedIndex;

        /// <summary>
        /// Stores the index of the static data instance that the user was looking at before hot reloading.
        /// </summary>
        private int typeInstanceSelectedIndex;

        /// <summary>
        /// Stores the type of the static data instance that the user was looking at before hot reloading.
        /// </summary>
        private Type selectedInstanceType;

        /// <summary>
        /// Listview of the currently selected data.
        /// </summary>
        private ListView selectedTypeListView;

        private const BindingFlags BindingFlagsToSelectStaticDataFields = BindingFlags.Instance | BindingFlags.Public;

        #region Styling

        private const float RowPadding = 4f;
        private static readonly StyleColor BorderColor = new(Color.gray);
        private static readonly StyleFloat BorderWidth = new(0.5f);

        #endregion

        #region Saving

        private JsonSerializer jsonSerializer;
        private static readonly string StaticDataDirectory = Path.Join(Application.dataPath, "StaticData");

        #endregion

        private void SetContext(OpenContext openContext)
        {
            this.openContext = openContext;
            CreateGUI();
        }

        public void CreateGUI()
        {
            if (openContext == OpenContext.None)
            {
                return;
            }

            jsonSerializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
            };
            jsonSerializer.Converters.Add(new AssetReferenceConverter());
            jsonSerializer.Converters.Add(new ColorConverter());
            jsonSerializer.ContractResolver = new CustomContractResolver();

            var root = rootVisualElement;

            var topToolBar = CreateTopToolBar();
            topToolBar.Add(CreateSaveAndExportJsonButton());
            topToolBar.Add(CreateSaveAndExportDatabaseButton());
            root.Add(topToolBar);

            LoadStaticDataTypes();
            CreateStaticDataTypeInstanceDictionary();

            var twoPanelSplit = new TwoPaneSplitView
            {
                fixedPaneInitialDimension = 140,
                orientation = TwoPaneSplitViewOrientation.Horizontal
            };
            var leftPanel = CreateLeftPanel();
            var rightPanel = new VisualElement();
            rightPanel.Add(CreateRightPanel(leftPanel));
            twoPanelSplit.Add(leftPanel);
            twoPanelSplit.Add(rightPanel);
            root.Add(twoPanelSplit);
        }

        private void LoadStaticDataTypes()
        {
            var iKeyableType = typeof(StaticData);
            staticDataTypes = iKeyableType.Assembly.GetTypes()
                .Where(type => iKeyableType.IsAssignableFrom(type) && type != iKeyableType)
                .ToList();
        }

        //TODO: Load from database
        private void CreateStaticDataTypeInstanceDictionary()
        {
            staticDataInstances ??= new Dictionary<Type, List<StaticData>>();

            // If we hot reload while the window is open and a new static data type is created, add to our dict
            foreach (var type in staticDataTypes)
            {
                if (!staticDataInstances.ContainsKey(type))
                {
                    var typeInstances = new List<StaticData>();
                    var typeDirectory = Path.GetFullPath(Path.Join(StaticDataDirectory, type.Name));

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

                    staticDataInstances.Add(type, typeInstances);
                }
            }
        }

        private async UniTask SaveAllStaticDataToJson()
        {
            var writeTasks = new List<UniTask>();
            foreach (var kvp in staticDataInstances)
            {
                var staticDataFilePath = Path.GetFullPath(Path.Join(StaticDataDirectory, kvp.Key.Name));
                MyLogger.Log($"Creating dir: {staticDataFilePath}");
                var directoryInfo = Directory.CreateDirectory(staticDataFilePath);

                foreach (var file in directoryInfo.GetDirectories())
                {
                    file.Delete();
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

            var button = new ToolbarButton(() =>
            {
                MyLogger.Log("Saving and exporting to json...");
                _ = SaveAllStaticDataToJson();
            })
            {
                text = "Save and Export to Json",
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

        private ListView CreateLeftPanel()
        {
            var listView = new ListView
            {
                makeItem = () => new Label(),
                bindItem = (item, index) => { ((Label)item).text = staticDataTypes[index].Name; },
                itemsSource = staticDataTypes,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All
            };

            listView.selectionChanged += _ => { selectedIndex = listView.selectedIndex; };

            return listView;
        }

        /// <param name="staticDataTypesListView">The list of static data types that the right panel reacts to.</param>
        /// <returns></returns>
        private VisualElement CreateRightPanel(ListView staticDataTypesListView)
        {
            Type selectedType;

            var root = new VisualElement();
            var label = new Label("Select a static data type.");

            staticDataTypesListView.selectionChanged += _ =>
            {
                selectedType = staticDataTypes?[selectedIndex];

                root.Clear();

                if (selectedType == null)
                {
                    return;
                }

                selectedTypeListView = CreateTypeInstancesListView(selectedType);
                root.Add(new Label($"{selectedType?.Name} Instances"));
                root.Add(new Button(() =>
                {
                    staticDataInstances[selectedType!].Add(Activator.CreateInstance(selectedType) as StaticData);
                    selectedTypeListView.RefreshItems();
                })
                {
                    text = $"Add new {selectedType?.Name}",
                    style = { width = 200 }
                });

                root.Add(CreateInstanceHeader(selectedType));
                root.Add(selectedTypeListView);
            };

            root.Add(label);

            return root;
        }

        private ListView CreateTypeInstancesListView(Type selectedType)
        {
            var listView = new ListView
            {
                makeItem = () => new VisualElement(),
                bindItem = (item, index) =>
                {
                    item.Clear();
                    item.Add(CreateInstanceRow(index, selectedType));
                },
                itemsSource = staticDataInstances[selectedType],
                showAlternatingRowBackgrounds = AlternatingRowBackground.All
            };

            listView.selectionChanged += _ =>
            {
                typeInstanceSelectedIndex = listView.selectedIndex;
                var shouldRefreshView = selectedType != selectedInstanceType && HasOpenInstances<InstanceStaticDataEditorWindow>();
                selectedInstanceType = selectedType;
                var instanceEditor = GetWindow<InstanceStaticDataEditorWindow>();

                if (shouldRefreshView)
                {
                    instanceEditor.rootVisualElement.Clear();
                    instanceEditor.CreateGUI();
                }

                instanceEditor.Show();
                instanceEditor.Focus();
            };

            return listView;
        }

        private Label CreateInstanceColumn(string labelText) => new(labelText)
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
                    borderBottomColor = BorderColor,
                    borderBottomWidth = BorderWidth,
                }
            };

            header.Add(CreateInstanceColumn("Index"));

            foreach (var field in staticDataType.GetFields(BindingFlagsToSelectStaticDataFields))
            {
                header.Add(CreateInstanceColumn($"{field.Name}"));
            }

            return header;
        }

        private VisualElement CreateInstanceRow(int index, Type staticDataType)
        {
            var row = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row }
            };

            var staticDataObj = staticDataInstances[staticDataType][index];

            row.Add(CreateInstanceColumn(index.ToString()));

            foreach (var field in staticDataType.GetFields(BindingFlagsToSelectStaticDataFields))
            {
                row.Add(CreateInstanceColumn($"{field.GetValue(staticDataObj)}"));
            }

            return row;
        }

        private class InstanceStaticDataEditorWindow : EditorWindow
        {
            private Type selectedType;
            private StaticData editingObj;

            private StaticDataEditorWindow openedEditorWindow;

            public void CreateGUI()
            {
                var root = rootVisualElement;
                openedEditorWindow = GetWindow<StaticDataEditorWindow>();

                selectedType = openedEditorWindow.selectedInstanceType;
                if (selectedType == null)
                {
                    MyLogger.LogError("Closing instance editor since selected type is null");
                    Close();
                    return;
                }

                editingObj = openedEditorWindow.staticDataInstances?[selectedType]?[openedEditorWindow.typeInstanceSelectedIndex];

                if (editingObj == null)
                {
                    MyLogger.LogError($"Closing instance editor object to edit is null for type {selectedType}");
                    Close();
                    return;
                }

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
                openedEditorWindow.staticDataInstances[selectedType][openedEditorWindow.typeInstanceSelectedIndex] = editingObj;
                openedEditorWindow.selectedTypeListView.RefreshItems();
                base.SaveChanges();
            }
        }
    }
}