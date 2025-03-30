using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tooling.Logging;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Utils.Extensions;
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
        private readonly System.Type type;

        /// <summary>
        /// How the value is retrieved from the underlying object.
        /// </summary>
        private readonly IValueProvider valueProvider;

        private readonly Options options;

        /// <summary>
        /// Invoked when the value that this field is drawing has changed.
        /// </summary>
        public event Action<object> OnValueChanged;

        /// <summary>
        /// Label when this is an array element, null otherwise.
        /// </summary>
        private Label arrayIndexLabel;

        /// <summary>
        /// The custom drawer for this visual element. See <see cref="DrawerManager"/> for usage.
        /// </summary>
        private readonly IDrawer drawer;

        /// <summary>
        /// The label to show when a <see cref="StaticData"/> reference is null.
        /// </summary>
        private const string StaticDataNullLabel = "null";

        public GeneralField(Type type, IValueProvider valueProvider, Options options = default)
        {
            this.type = type;
            this.valueProvider = valueProvider;
            this.options = options;

            AddToClassList(VisualElementClasses.RecursiveFieldContainer);

            drawer = DrawerManager.Instance.Drawers.GetValueOrDefault(type);

            RefreshView();
        }

        private void RefreshView()
        {
            Clear();

            if (options.IsArrayElement && valueProvider is ListValueProvider listValueProvider)
            {
                arrayIndexLabel = new Label($"[{listValueProvider.ArrayIndex}]");
                var rowElement = new VisualElement();
                rowElement.Add(arrayIndexLabel);
                rowElement.Add(DrawEditorForType(type));

                rowElement.AddToClassList(VisualElementClasses.ListViewContainer);
                Add(rowElement);
            }
            else
            {
                Add(DrawEditorForType(type));
            }
        }

        /// <summary>
        /// Draws an editor field for the given type.
        /// </summary>
        private VisualElement DrawEditorForType(System.Type type)
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
                editorForFieldType = drawer.Draw(GetValue, SetValue, valueProvider.ValueName);
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                editorForFieldType = CreateListField(type);
            }
            else if (!options.EnumerateStaticDataProperties && typeof(StaticData).IsAssignableFrom(type))
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
            else
            {
                editorForFieldType = RecursiveDrawElements(type);
            }

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
                GetEnumName);

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
        private VisualElement CreateListField(System.Type type)
        {
            var root = new VisualElement();
            root.AddToClassList(VisualElementClasses.ListView);

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
                makeItem = () => new GeneralField(
                    elementType,
                    new ListValueProvider(itemsSource.Count - 1, itemsSource),
                    new Options { IsArrayElement = true }),
                bindItem = (item, index) => ((GeneralField)item).BindArrayIndex(index),
                unbindItem = (item, _) => ((GeneralField)item).BindArrayIndex(-1),
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                reorderable = true,
                showBorder = true,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderMode = ListViewReorderMode.Animated,
                showFoldoutHeader = true,
                headerTitle = valueProvider?.ValueName ?? string.Empty,
                showAddRemoveFooter = false,
                showBoundCollectionSize = false,
                horizontalScrollingEnabled = true
            };

            listView.itemsAdded += _ => OnValueChanged?.Invoke(itemsSource);
            listView.itemsRemoved += _ => OnValueChanged?.Invoke(itemsSource);
            listView.itemsSourceChanged += () => OnValueChanged?.Invoke(itemsSource);
            listView.itemIndexChanged += (_, _) => OnValueChanged?.Invoke(itemsSource);

            root.Add(listView);

            var buttonContainer = new VisualElement();

            var listViewFoldOut = listView.Q<Foldout>(name: "unity-list-view__foldout-header");
            listViewFoldOut.RegisterValueChangedCallback(evt => ShowAddRemoveButtons(evt.newValue));
            ShowAddRemoveButtons(listViewFoldOut.value);

            buttonContainer.AddToClassList(VisualElementClasses.ListViewButtonContainer);

            buttonContainer.Add(new Button(() =>
            {
                itemsSource.Add(GetDefaultValue(elementType));
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

            void ShowAddRemoveButtons(bool show)
            {
                if (show)
                {
                    root.Add(buttonContainer);
                }
                else
                {
                    root.RemoveIfChild(buttonContainer);
                }
            }
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

            RefreshView();
        }

        private VisualElement CreateStaticDataField(System.Type type)
        {
            var root = new VisualElement();

            root.AddToClassList(VisualElementClasses.StaticDataSelectorContainer);

            var label = new Label(valueProvider.ValueName);
            label.AddToClassList("unity-base-field__label");
            root.Add(label);

            var nameLabel = new Label((GetValue() as StaticData)?.Name ?? StaticDataNullLabel);

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
            root.AddToClassList(VisualElementClasses.RecursiveFieldContainer);
            var fields = Utils.GetFields(type);

            // Move name to the top of our editor for static data types
            // while keeping the order of the rest of the fields
            if (typeof(StaticData).IsAssignableFrom(type) && fields.Count > 1)
            {
                var nameField = fields.First(field => field.Name == nameof(StaticData.Name));
                var nameIndex = fields.IndexOf(nameField);
                for (int i = fields.Count - 1; i > 0; i--)
                {
                    if (i > nameIndex)
                    {
                        continue;
                    }

                    fields[i] = fields[i - 1];
                }

                fields[0] = nameField;
            }

            foreach (var field in fields)
            {
                var generalField = new GeneralField(field.FieldType, new FieldValueProvider(field, currentObj));
                generalField.OnValueChanged += obj => OnValueChanged?.Invoke(obj);

                if (Utils.GetFields(field.FieldType).Count > 1 && !typeof(StaticData).IsAssignableFrom(field.FieldType))
                {
                    var foldout = new Foldout();
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
                    : t.FullName);

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

                SetValue(shouldCreateNewInstance
                    ? Activator.CreateInstance(selectedType)
                    : GetDefaultValue(selectedType)
                );
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
            var root = new VisualElement();
            root.Add(new Label(valueProvider.ValueName));

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

        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Used internally by the <see cref="GeneralField"/> to draw list elements.
        /// </summary>
        private class ListValueProvider : IValueProvider
        {
            private readonly IList list;

            private int arrayIndex;

            public int ArrayIndex
            {
                get => arrayIndex;
                set
                {
                    arrayIndex = value;
                    
                    // Not sure if I love this approach is it tightly couples GeneralField and IInstructions,
                    // but it gets the job done
                    if (arrayIndex > 0 && arrayIndex < list.Count && list[arrayIndex] is IInstruction instruction)
                    {
                        instruction.Index = arrayIndex;
                    }
                }
            }

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

    public struct Options
    {
        /// <summary>
        /// If true, we'll draw the array index label.
        /// </summary>
        public bool IsArrayElement;

        /// <summary>
        /// If true, we'll draw static data like every other element instead of using the object picker.
        /// </summary>
        public bool EnumerateStaticDataProperties;
    }


    public class GeneralField<T> : GeneralField
    {
        public GeneralField(IValueProvider valueProvider, Options options = default) : base(typeof(T), valueProvider, options)
        {
        }
    }
}