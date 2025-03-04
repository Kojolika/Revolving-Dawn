using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tooling.Logging;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Tooling.StaticData.EditorUI
{
    /// <summary>
    /// A property field that supports multiple types.
    /// </summary>
    public class GeneralField : VisualElement
    {
        /// <summary>
        /// The type this field is drawing.
        /// </summary>
        private readonly Type type;

        /// <summary>
        /// How the value is retrieved from the underlying object.
        /// </summary>
        private readonly IValueProvider valueProvider;

        private readonly bool drawArrayLabel;

        /// <summary>
        /// Invoked when the value that this field is drawing has changed.
        /// </summary>
        public event Action<object> OnValueChanged;

        /// <summary>
        /// Label when this is an array element, null otherwise.
        /// </summary>
        private Label arrayIndexLabel;

        /// <summary>
        /// The custom decorator for this visual element. See <see cref="DecoratorManager"/> for usage.
        /// </summary>
        private readonly IDecorator decorator;

        /// <summary>
        /// The custom drawer for this visual element. See <see cref="DrawerManager"/> for usage.
        /// </summary>
        private readonly IDrawer drawer;

        /// <summary>
        /// The label to show when a <see cref="StaticData"/> reference is null.
        /// </summary>
        private const string StaticDataNullLabel = "null";

        /// <summary>
        /// The name of the <see cref="VisualElement"/> that is actually drawing the <see cref="type"/>
        /// </summary>
        private const string FieldDrawerName = "FieldDrawer";

        public GeneralField(Type type, IValueProvider valueProvider, bool drawArrayLabel = false)
        {
            this.type = type;
            this.valueProvider = valueProvider;
            this.drawArrayLabel = drawArrayLabel;

            decorator = DecoratorManager.Instance.Decorators.GetValueOrDefault(type);
            drawer = DrawerManager.Instance.Drawers.GetValueOrDefault(type);
            OnValueChanged += _ =>
            {
                decorator?.Dispose(this);
                decorator?.DecorateElement(this, GetValue());
            };

            RefreshView();
        }

        private void RefreshView()
        {
            Clear();

            style.flexDirection = FlexDirection.Column;

            if (drawArrayLabel && valueProvider is ListValueProvider listValueProvider)
            {
                arrayIndexLabel = new Label(listValueProvider.ArrayIndex.ToString())
                {
                    style =
                    {
                        minWidth = 16,
                        alignSelf = Align.FlexStart,
                        alignItems = Align.Center,
                        alignContent = Align.Center,
                        unityTextAlign = TextAnchor.MiddleCenter,
                        marginTop = 1
                    }
                };
                var rowElement = new VisualElement { style = { flexDirection = FlexDirection.Row } };
                rowElement.Add(arrayIndexLabel);
                rowElement.Add(DrawEditorForType(type));
                Add(rowElement);
            }
            else
            {
                Add(DrawEditorForType(type));
            }

            decorator?.DecorateElement(this, GetValue());
        }

        /// <summary>
        /// Draws an editor field for the given type.
        /// </summary>
        private VisualElement DrawEditorForType(Type type)
        {
            if (type == null)
            {
                return new Label($"Passed null type to {nameof(DrawEditorForType)}");
            }

            if (type.GetCustomAttribute<GeneralFieldIgnoreAttribute>()?.IgnoreType == IgnoreType.Field)
            {
                return new VisualElement();
            }

            VisualElement editorForFieldType;
            if (drawer != null)
            {
                editorForFieldType = drawer.Draw(GetValue, obj => SetValue(obj), valueProvider.ValueName);
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                editorForFieldType = CreateListField(type);
            }
            else if (typeof(StaticData).IsAssignableFrom(type))
            {
                editorForFieldType = CreateStaticDataField(type);
            }
            else if (typeof(Enum).IsAssignableFrom(type))
            {
                editorForFieldType = CreateEnumField(type);
            }
            else if (type.IsInterface || type.IsAbstract)
            {
                editorForFieldType = CreateAbstractTypeSelection(type);
            }
            else if (typeof(AssetReference).IsAssignableFrom(type))
            {
                editorForFieldType = CreateAssetReferenceObjectPicker(type);
            }
            else if (type.GetCustomAttribute<SerializableAttribute>() is not null)
            {
                editorForFieldType = RecursiveDrawElements(type);
            }
            else
            {
                editorForFieldType = GetErrorElement(type);
            }

            editorForFieldType.name = FieldDrawerName;


            return editorForFieldType;
        }

        private VisualElement CreateEnumField(Type type)
        {
            var enumValues = Enum.GetValues(type).Cast<Enum>().ToList();
            var popupField = new PopupField<Enum>(
                valueProvider.ValueName,
                enumValues,
                enumValues.FirstOrDefault(),
                GetEnumName,
                GetEnumName)
            {
                style = { alignSelf = Align.FlexStart }
            };

            popupField.RegisterValueChangedCallback(evt => SetValue(evt.newValue));

            return popupField;

            // Returns the name of an Enum value or the overriden name
            string GetEnumName(Enum value)
            {
                var valueName = Enum.GetName(type, value) ?? string.Empty;
                return type.GetMember(valueName)
                           .First()
                           .GetCustomAttribute<DisplayNameAttribute>() is var displayNameAttribute
                       && !string.IsNullOrEmpty(displayNameAttribute?.Name)
                    ? displayNameAttribute.Name
                    : valueName;
            }
        }

        /// <summary>
        /// Creates a custom list view field that draws GeneralFields as its listview elements
        /// </summary>
        /// <returns>A visual element containing the list view.</returns>
        /// <exception cref="ArgumentException">Fired if the fieldInfo is not a type of <see cref="IList"/></exception>
        private VisualElement CreateListField(Type type)
        {
            var root = new VisualElement { style = { flexDirection = FlexDirection.Row, minWidth = 200 } };
            // If the type is an array
            var elementType = type.IsArray
                ? type.GetElementType()
                // otherwise if it's a generic list
                : type.IsGenericType
                    ? type.GetGenericArguments()[0]
                    : throw new ArgumentException($"Cannot find element type on {type}");

            var itemsSource = GetValue() as IList
                              ?? (IList)Activator.CreateInstance(type);

            var listView = new ListView
            {
                itemsSource = itemsSource,
                makeItem = () => new GeneralField(elementType, new ListValueProvider(itemsSource.Count - 1, itemsSource), true),
                bindItem = (item, index) => ((GeneralField)item).BindArrayIndex(index),
                unbindItem = (item, _) => ((GeneralField)item).BindArrayIndex(-1),
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                reorderable = true,
                showBorder = true,
                style = { minWidth = 150, display = DisplayStyle.Flex },
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderMode = ListViewReorderMode.Animated,
                showFoldoutHeader = true,
                horizontalScrollingEnabled = true,
                headerTitle = valueProvider?.ValueName ?? string.Empty
            };

            listView.Query(className: "unity-foldout__input").First().style.backgroundColor = new Color(0, 0, 0, 0.33f);

            listView.itemsAdded += _ => OnValueChanged?.Invoke(itemsSource);
            listView.itemsRemoved += _ => OnValueChanged?.Invoke(itemsSource);
            listView.itemsSourceChanged += () => OnValueChanged?.Invoke(itemsSource);
            listView.itemIndexChanged += (_, _) => OnValueChanged?.Invoke(itemsSource);

            root.Add(listView);

            var buttonContainer = new VisualElement { style = { flexDirection = FlexDirection.Column } };
            root.Add(buttonContainer);
            buttonContainer.Add(new Button(() =>
            {
                itemsSource.Add(GetDefault(elementType));
                SetValue(itemsSource);
                listView.RefreshItems();
            })
            {
                text = "+"
            });

            buttonContainer.Add(new Button(() =>
            {
                var selectedIndex = listView.selectedIndex;
                selectedIndex = selectedIndex == -1
                    ? itemsSource.Count - 1
                    : selectedIndex;

                if (selectedIndex < 0)
                {
                    return;
                }

                itemsSource.RemoveAt(selectedIndex);
                SetValue(itemsSource);
                listView.RefreshItems();
            })
            {
                text = "-"
            });

            return root;
        }

        /// <summary>
        /// Only applies if this is drawing an element in a list. Notifies the editor field that index
        /// is where the value should get retrieved from.
        /// </summary>
        /// <param name="index"></param>
        private void BindArrayIndex(int index)
        {
            if (index < 0)
            {
                return;
            }

            ((ListValueProvider)valueProvider).ArrayIndex = index;
            arrayIndexLabel.text = index.ToString();

            RefreshView();
        }

        private VisualElement CreateStaticDataField(Type type)
        {
            var root = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.FlexStart } };
            root.Add(GetDefaultNameLabel(valueProvider.ValueName));

            var nameLabel = new Label((GetValue() as StaticData)?.Name ?? StaticDataNullLabel)
            {
                style = { alignSelf = Align.Center, minWidth = 40 }
            };

            root.Add(
                new ButtonIcon(
                    () =>
                    {
                        var instanceSelector = InstancesView.Selector.Open(type);
                        instanceSelector.onSelectionChanged += staticData => OnValueChanged?.Invoke(staticData);
                    },
                    IconPaths.List
                )
            );
            root.Add(nameLabel);

            if (!type.IsInstanceOfType(GetValue()))
            {
                SetValue(null);
            }

            return root;
        }

        /// <summary>
        /// Similar to how the inspector draws serializable types. Draws all serializable fields of the given type.
        /// </summary>
        private VisualElement RecursiveDrawElements(Type type)
        {
            var currentObj = GetValue();
            if (GetValue() == null)
            {
                currentObj = Activator.CreateInstance(type);
                SetValue(currentObj);
            }

            var root = new VisualElement();
            foreach (var field in Utils.GetFields(type))
            {
                var generalField = new GeneralField(field.FieldType, new FieldValueProvider(field, currentObj));
                generalField.OnValueChanged += obj => OnValueChanged?.Invoke(obj);

                if (!type.IsValueType)
                {
                    var foldout = new Foldout();
                    foldout.Query(className: "unity-foldout__input").First().style.backgroundColor = new Color(0, 0, 0, 0.33f);
                    foldout.Add(generalField);
                    root.Add(foldout);
                }
                else
                {
                    root.Add(generalField);
                }
            }

            return root;
        }

        private VisualElement CreateAbstractTypeSelection(Type type)
        {
            var root = new VisualElement();
            var concreteTypes = type.Assembly.DefinedTypes
                .Where(t => type.IsAssignableFrom(t)
                            && !t.IsAbstract
                            && !t.IsInterface
                            && t.GetCustomAttribute<GeneralFieldIgnoreAttribute>() is not { IgnoreType: IgnoreType.Interface }
                            && t.GetInterfaces()
                                .All(tInterface => tInterface.GetCustomAttribute<GeneralFieldIgnoreAttribute>() == null))
                .Select(tInfo => tInfo.AsType())
                .ToList();

            if (concreteTypes.Count < 1)
            {
                return new Label($"No types inherit from {type}");
            }

            var currentValueType = GetValue()?.GetType();
            var typeToDisplay = concreteTypes.Contains(currentValueType)
                ? currentValueType
                : concreteTypes.First();

            var popupField = new PopupField<Type>(
                valueProvider?.ValueName ?? string.Empty,
                concreteTypes,
                typeToDisplay,
                t => t.GetCustomAttribute<DisplayNameAttribute>() is { Name: not null } prettifyNameAttribute
                    ? prettifyNameAttribute.Name
                    : t.FullName,
                t => t.GetCustomAttribute<DisplayNameAttribute>() is { Name: not null } prettifyNameAttribute
                    ? prettifyNameAttribute.Name
                    : t.FullName)
            {
                style = { alignSelf = Align.FlexStart }
            };

            root.Add(popupField);

            var interfaceField = CreateGeneralFieldForInterfaceType(typeToDisplay);
            root.Add(interfaceField);

            popupField.RegisterValueChangedCallback(evt =>
            {
                typeToDisplay = evt.newValue;
                if (typeToDisplay == null)
                {
                    MyLogger.LogError($"Selected type {evt.newValue} but cannot find type in assembly!");
                    return;
                }

                OnTypeSelected(typeToDisplay, evt.previousValue);

                root.Remove(interfaceField);
                interfaceField = CreateGeneralFieldForInterfaceType(typeToDisplay);
                root.Add(interfaceField);
            });

            OnTypeSelected(typeToDisplay, currentValueType);

            return root;

            GeneralField CreateGeneralFieldForInterfaceType(Type newType)
            {
                return new GeneralField(newType, valueProvider);
            }

            void OnTypeSelected(Type selectedType, Type previousSelectedType)
            {
                // Allow creating a default instance for objects
                // We don't need this for unity engine objects or static data since we can select instances with the general field
                var shouldCreateNewInstance = (GetValue() == null || selectedType != previousSelectedType) &&
                                              (selectedType.IsValueType || selectedType.GetConstructor(Type.EmptyTypes) != null) &&
                                              !typeof(StaticData).IsAssignableFrom(typeToDisplay) &&
                                              !typeof(Object).IsAssignableFrom(typeToDisplay);

                if (shouldCreateNewInstance)
                {
                    SetValue(Activator.CreateInstance(selectedType));
                }
            }
        }

        /// <summary>
        /// Supports drawing an object picker for <see cref="AssetReference"/> and <see cref="AssetReferenceT{TObject}"/>
        /// as well as any AssetReferences that inherit from <see cref="AssetReferenceT{TObject}"/>.
        /// <remarks>
        /// Does not support custom generic classes that inherit from <see cref="AssetReferenceT{TObject}"/>
        /// </remarks>
        /// </summary>
        private VisualElement CreateAssetReferenceObjectPicker(Type type)
        {
            var root = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            root.Add(GetDefaultNameLabel(valueProvider.ValueName));

            var isGenericAssetReference = false;
            var assetReferenceType = type;

            while (assetReferenceType != typeof(AssetReference) && assetReferenceType != null && !isGenericAssetReference)
            {
                isGenericAssetReference = assetReferenceType.IsGenericType;
                if (!isGenericAssetReference)
                {
                    assetReferenceType = assetReferenceType.BaseType;
                }
            }

            var objectType = isGenericAssetReference
                ? assetReferenceType.GetGenericArguments()[0]
                : typeof(Object);

            if (!typeof(Object).IsAssignableFrom(objectType))
            {
                MyLogger.LogError($"Invalid type {type}, cannot find asset reference type {objectType} because it does not" +
                                  $"derive from {typeof(Object)}");
                return null;
            }

            var objectPicker = new ObjectField
            {
                objectType = objectType,
                style = { marginLeft = 0 },
                value = (GetValue() as AssetReference)?.editorAsset
            };

            objectPicker.RegisterValueChangedCallback(evt =>
            {
                var assetPath = AssetDatabase.GetAssetPath(evt.newValue);
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                var assetReference = Activator.CreateInstance(type, guid);
                SetValue(assetReference);
            });

            root.Add(objectPicker);

            return root;
        }

        /// <summary>
        /// Creates a label field with the same default values as built in labels
        /// </summary>
        private Label GetDefaultNameLabel(string name)
        {
            var label = new Label(name);

            label.AddToClassList("unity-text-element");
            label.AddToClassList("unity-label");
            label.AddToClassList("unity-base-field__label");
            label.AddToClassList("unity-base-text-field__label");
            label.AddToClassList("unity-text-field__label");

            return label;
        }

        /// <summary>
        /// Sets the value on the underlying instance this field is bound to.
        /// </summary>
        private void SetValue(object value)
        {
            valueProvider.SetValue(value);
            OnValueChanged?.Invoke(value);
        }

        /// <summary>
        /// Gets the value on the underlying instance this field is bound to.
        /// </summary>
        private object GetValue()
        {
            return valueProvider.GetValue();
        }

        /// <summary>
        /// Returns what to draw when the specified type cannot be drawn.
        /// </summary>
        private static VisualElement GetErrorElement(Type type)
        {
            return new Label($"No editor created for type {type}");
        }

        /// <summary>
        /// Returns the <see cref="VisualElement"/> that is actually drawing the <see cref="type"/>
        /// If this is drawing an array, you may want to just use the generalfield itself instead of this.
        /// </summary>
        public VisualElement GetFieldDrawer()
        {
            return this.Query<VisualElement>(FieldDrawerName).First();
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        /// <summary>
        /// Used internally by the <see cref="GeneralField"/> to draw list elements.
        /// </summary>
        private class ListValueProvider : IValueProvider
        {
            private readonly IList list;
            public int ArrayIndex { get; set; }
            public string ValueName => string.Empty;

            public ListValueProvider(int arrayIndex, IList list)
            {
                this.list = list;
                ArrayIndex = arrayIndex;
            }

            public void SetValue(object value)
            {
                list[ArrayIndex] = value;
            }

            public object GetValue()
            {
                return ArrayIndex >= list.Count || ArrayIndex < 0
                    ? null
                    : list[ArrayIndex];
            }
        }
    }
}