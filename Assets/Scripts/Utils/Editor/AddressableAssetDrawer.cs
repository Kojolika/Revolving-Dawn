using Tooling.Logging;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Attributes;


[CustomPropertyDrawer(typeof(AddressableAssetAttribute))]
public class AddressableAssetDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var attr = attribute as AddressableAssetAttribute;

        var root = new VisualElement();

        MyLogger.Log($" IS array? {property.isArray}");

        var objectField = new ObjectField
        {
            objectType = attr.Type,
            tooltip = "Selected object will have its addressable asset extracted and put into the string field, if no addressable asset key exists an error will be shown below"
        };

        var button = new Button
        {
            text = "Toggle AddressableAsset Field",
            tooltip = "Toggles showing the object field for an addressable asset"
        };
        button.clicked += () =>
        {
            if (root.Contains(objectField))
            {
                root.Remove(objectField);
            }
            else
            {
                root.Insert(1, objectField);
            }
        };

        var propertyField = new PropertyField
        {
            visible = property.stringValue != null || property.stringValue != string.Empty
        };
        propertyField.BindProperty(property);

        var addressableAssetAddressMissingLabel = new Label()
        {
            text = "Object doesn't have an addressable key!",
            visible = false
        };

        objectField.RegisterValueChangedCallback((ChangeEvent<UnityEngine.Object> changeEvent) =>
        {
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(changeEvent.newValue, out var guid, out long localId)
                && AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid) is AddressableAssetEntry addressableAssetEntry
                && addressableAssetEntry != null)
            {
                propertyField.visible = true;

                property.serializedObject.Update();
                property.stringValue = addressableAssetEntry.address;
                property.serializedObject.ApplyModifiedProperties();
                MyLogger.Log($"array size: {property.arraySize}");

                addressableAssetAddressMissingLabel.visible = false;
            }
            else
            {
                propertyField.visible = false;
                addressableAssetAddressMissingLabel.visible = true;
            }
        });

        root.Add(button);
        root.Add(propertyField);
        root.Add(addressableAssetAddressMissingLabel);

        root.style.width = 400;

        var borderWidth = 1;
        var borderColor = Color.white;
        root.style.borderBottomWidth = borderWidth;
        root.style.borderBottomColor = borderColor;

        root.style.borderTopWidth = borderWidth;
        root.style.borderTopColor = borderColor;

        root.style.borderLeftWidth = borderWidth;
        root.style.borderLeftColor = borderColor;

        root.style.borderRightWidth = borderWidth;
        root.style.borderRightColor = borderColor;

        var padding = 10;
        root.style.paddingLeft = padding;
        root.style.paddingRight = padding;

        return root;
    }
}