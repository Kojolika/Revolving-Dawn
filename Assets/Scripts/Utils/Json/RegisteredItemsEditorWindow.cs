using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Utils.Json
{
    public class RegisteredItemsEditorWindow : EditorWindow
    {
        private const int ButtonWidth = 120;

        private ListView listView;
        private List<VisualElement> listItems;

        private void OnEnable()
        {
            listItems = GetListItems();
            listView = new ListView(listItems, makeItem: CreateListItem)
            {
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBoundCollectionSize = true,
                showAddRemoveFooter = true,
            };
            Debug.Log("OnEnable");
        }

        [MenuItem("KoJy/Json/RegisteredItems")]
        private static void OpenRegisteredJsonWindow()
        {
            var editorWindow = GetWindow<RegisteredItemsEditorWindow>();
            editorWindow.titleContent = new GUIContent("Registered JSON Items");
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            root.Add(GetOptionsRow());
            root.Add(listView);
        }

        private List<VisualElement> GetListItems()
        {
            //TODO: get from json
            var list = new List<VisualElement>();

            return list;
        }

        private VisualElement GetOptionsRow()
        {
            var row = new VisualElement();
            row.Add(GetSaveButton());
            row.Add(GetDiscardButton());
            row.Add(GetAddButton());

            row.style.flexDirection = FlexDirection.Row;
            row.style.backgroundColor = Color.gray;

            return row;
        }

        private Button GetAddButton()
        {
            var button = new Button(AddNewItem);

            button.Add(new Label("Add new item"));
            button.style.width = ButtonWidth;

            return button;
        }

        private void AddNewItem()
        {
            listView?.Insert(0, CreateListItem());
            //listItems.Add(GetListItem());
            Debug.Log("Adding new entry");
        }

        private Button GetSaveButton()
        {
            var button = new Button(SaveChanges);

            button.Add(new Label("Save Changes"));
            button.style.width = ButtonWidth;

            return button;
        }
        private Button GetDiscardButton()
        {
            var button = new Button(DiscardChanges);

            button.Add(new Label("Discard Changes"));
            button.style.width = ButtonWidth;

            return button;
        }

        private VisualElement CreateListItem()
        {
            var root = new VisualElement();
            root.Add(new Label("Type"));
            root.Add(new TextField());
            return root;
        }
    }
}