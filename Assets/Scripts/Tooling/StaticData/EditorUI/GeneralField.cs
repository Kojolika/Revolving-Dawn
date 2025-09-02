using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Util;
using Tooling.Logging;
using Tooling.StaticData.Data.Validation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Utils.Extensions;
using Object = UnityEngine.Object;

namespace Tooling.StaticData.Data.EditorUI
{
    /// <summary>
    /// A property field that supports multiple types.
    /// </summary>
    public class GeneralField : VisualElement, INotifyValueChanged<object>
    {
        /// <summary>
        /// The type this field is drawing.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// How the value is retrieved from the underlying object.
        /// </summary>
        private readonly IValueProvider valueProvider;

        /// <summary>
        /// <see cref="INotifyValueChanged{T}"/> interface implementation
        /// </summary>
        public object value
        {
            get => GetValue();
            set => SetValueAndNotify(value);
        }

        /// <summary>
        /// Internal options used to draw the field
        /// </summary>
        private readonly Options options;

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
            this.Type          = type;
            this.valueProvider = valueProvider;
            this.options       = options;

            drawer = DrawerManager.Instance.Drawers.GetValueOrDefault(type);

            RefreshView();
        }

        private void RefreshView()
        {
            Clear();

            if (options.IsArrayElement && valueProvider is ListValueProvider listValueProvider)
            {
                var rowElement = new VisualElement();
                Utils.AddLabel(rowElement, $"[{listValueProvider.ArrayIndex}]", valueProvider);
                rowElement.Add(DrawEditorForType(Type));

                rowElement.AddToClassList(Styles.ListViewContainer);
                Add(rowElement);
            }
            else
            {
                Add(DrawEditorForType(Type));
            }
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
                editorForFieldType = drawer.Draw(valueProvider, this);
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
            else if (type == typeof(Type))
            {
                editorForFieldType = CreateTypeField();
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

        private PopupField<Enum> CreateEnumField(Type type)
        {
            var enumValues = Enum.GetValues(type).Cast<Enum>().ToList();
            var popupField = new PopupField<Enum>(
                valueProvider.ValueName,
                enumValues,
                (GetValue() ?? GetDefaultValue(type)) as Enum,
                GetEnumName,
                GetEnumName);

            popupField.RegisterValueChangedCallback(evt => { SetValueAndNotify(evt.newValue); });

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
        /// Creates a dropdown field to select a type that's in the current project.
        /// </summary>
        private PopupField<Type> CreateTypeField()
        {
            var typeValues = GetPossibleTypes(valueProvider);
            var popupField = new PopupField<Type>(
                valueProvider.ValueName,
                typeValues,
                (GetValue() ?? typeValues.FirstOrDefault()) as Type,
                formatListItemCallback: selectedType => selectedType?.FullName?.Replace('.', '/') ?? string.Empty);

            popupField.RegisterValueChangedCallback(evt => { SetValueAndNotify(evt.newValue); });

            return popupField;
        }

        /// <summary>
        /// Gets the list of selectable types for a field with a type of 'System.Type'
        /// </summary>
        /// <param name="valueProvider"> The value provider this general field is using</param>
        /// <returns> The list of possible types to use </returns>
        private static List<Type> GetPossibleTypes(IValueProvider valueProvider)
        {
            var typeValues = Assembly.GetCallingAssembly().GetTypes().ToList();

            // If possible, filter types by the attribute on our field values
            if (valueProvider is FieldValueProvider fieldValueProvider
             && fieldValueProvider.FieldInfo.GetCustomAttribute<IsAssignableFromAttribute>() is var isAssignableFromAttribute)
            {
                typeValues = typeValues.Where(t => isAssignableFromAttribute.TargetType.IsAssignableFrom(t)).ToList();
            }

            return typeValues;
        }

        /// <summary>
        /// Creates a custom list view field that draws GeneralFields as its listview elements
        /// </summary>
        /// <returns>A visual element containing the list view.</returns>
        /// <exception cref="ArgumentException">Fired if the fieldInfo is not a type of <see cref="IList"/></exception>
        private VisualElement CreateListField(Type type)
        {
            var root = new VisualElement();
            root.AddToClassList(Styles.ListView);

            // If the type is an array
            var elementType = type.IsArray
                ? type.GetElementType()
                // otherwise if it's a generic list
                : type.IsGenericType
                    ? type.GetGenericArguments()[0]
                    : typeof(object);

            var itemsSource = GetValue() as IList ?? (IList)Activator.CreateInstance(type);
            SetValueWithoutNotify(itemsSource);

            var listView = new ListView
            {
                itemsSource = itemsSource,
                makeItem = () => new GeneralField(
                    elementType,
                    new ListValueProvider(itemsSource.Count - 1, itemsSource),
                    new Options { IsArrayElement = true }),
                bindItem                      = BindItem,
                unbindItem                    = UnbindItem,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                reorderable                   = true,
                showBorder                    = true,
                virtualizationMethod          = CollectionVirtualizationMethod.DynamicHeight,
                reorderMode                   = ListViewReorderMode.Animated,
                showFoldoutHeader             = true,
                headerTitle                   = valueProvider?.ValueName ?? string.Empty,
                showAddRemoveFooter           = true,
                showBoundCollectionSize       = false,
                horizontalScrollingEnabled    = true
            };

            listView.itemsAdded += indices =>
            {
                foreach (var index in indices)
                {
                    listView.itemsSource[index] = GetDefaultValue(elementType);
                }

                listView.RefreshItems();
                SetValueAndNotify(listView.itemsSource);
            };
            listView.itemsRemoved       += _ => SetValueAndNotify(listView.itemsSource);
            listView.itemsSourceChanged += () => SetValueAndNotify(listView.itemsSource);
            listView.itemIndexChanged   += (_, _) => SetValueAndNotify(listView.itemsSource);

            root.Add(listView);

            return root;

            void BindItem(VisualElement item, int index)
            {
                var generalField = (GeneralField)item;
                generalField.BindListElement(index);
            }

            void UnbindItem(VisualElement item, int index)
            {
                var generalField = (GeneralField)item;
                generalField.BindListElement(-1);
            }
        }

        /// <summary>
        /// Only applies if this is drawing an element in a list. Notifies the editor field that index
        /// is where the value should get retrieved from.
        /// </summary>
        /// <param name="index"></param>
        private void BindListElement(int index)
        {
            if (index < 0)
            {
                return;
            }

            ((ListValueProvider)valueProvider).ArrayIndex = index;

            RefreshView();
        }

        private VisualElement CreateStaticDataField(Type type)
        {
            var root = new VisualElement();
            root.AddToClassList(Styles.StaticDataSelectorContainer);

            var selectedStaticData = GetValue() as StaticData;

            Utils.AddLabel(root, valueProvider.ValueName, valueProvider);
            Utils.AddLabel(root, selectedStaticData?.Name ?? StaticDataNullLabel, valueProvider);

            var editButton = new ButtonIcon(
                clickEvent: () =>
                {
                    InstancesTable.Selector.Open(
                        staticDataType: type,
                        onSelectionChanged: staticData =>
                        {
                            SetValueAndNotify(staticData);
                            RefreshView();
                        });
                },
                iconPath: IconPaths.List
            );
            root.Add(editButton);

            if (selectedStaticData != null)
            {
                var redirectButton = new ButtonIcon(
                    clickEvent: () => { EditorWindow.InstanceEditorWindow.Open(selectedStaticData, selectedStaticData.GetType()); },
                    iconPath: IconPaths.Redirect);
                root.Add(redirectButton);
            }

            /*
            if (!type.IsInstanceOfType(GetValue()))
            {
                SetValue(null);
            }
            */

            return root;
        }

        /// <summary>
        /// Similar to how the inspector draws serializable types. Draws all serializable fields of the given type.
        /// </summary>
        private VisualElement RecursiveDrawElements(Type type)
        {
            var currentObj = GetValue();
            if (GetValue() == null && type.GetConstructor(Type.EmptyTypes) != null)
            {
                currentObj = Activator.CreateInstance(type);
                SetValueAndNotify(currentObj);
            }

            var root = new VisualElement();
            root.AddToClassList(Styles.AlignStart);

            var fields = Utils.GetFields(type);
            Utils.SortFields(fields, type);

            foreach (var field in fields)
            {
                var generalField = new GeneralField(field.FieldType, new FieldValueProvider(field, currentObj));
                generalField.RegisterValueChangedCallback(evt => { SendEvent(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue)); });

                if (Utils.GetFields(field.FieldType).Count > 1
                 && !typeof(StaticData).IsAssignableFrom(field.FieldType)) // Hacky - out static data has a custom editor where we don't want a foldout
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
                    MyLogger.Error($"Selected type {evt.newValue} but cannot find type in assembly!");
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
                var shouldCreateNewInstance = (GetValue() == null || selectedType != previousSelectedType)
                                           && (selectedType.IsValueType || selectedType.GetConstructor(Type.EmptyTypes) != null)
                                           && (!typeof(StaticData).IsAssignableFrom(typeToDisplay)
                                            || options.EnumerateStaticDataProperties)
                                           && !typeof(Object).IsAssignableFrom(typeToDisplay);

                if (shouldCreateNewInstance)
                {
                    SetValueAndNotify(Activator.CreateInstance(selectedType));
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
            var root = new VisualElement();
            root.AddToClassList(Styles.FlexRow);

            Utils.AddLabel(root, valueProvider.ValueName, valueProvider);

            var isGenericAssetReference = false;
            var assetReferenceType      = type;

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
                MyLogger.Error($"Invalid type {type}, cannot find asset reference type {objectType} because it does not" +
                               $"derive from {typeof(Object)}");
                return null;
            }

            var objectPicker = new ObjectField
            {
                objectType = objectType,
                value      = (GetValue() as AssetReference)?.editorAsset
            };

            objectPicker.RegisterValueChangedCallback(evt =>
            {
                var assetPath      = AssetDatabase.GetAssetPath(evt.newValue);
                var guid           = AssetDatabase.AssetPathToGUID(assetPath);
                var assetReference = Activator.CreateInstance(type, guid);
                SetValueAndNotify(assetReference);
            });

            root.Add(objectPicker);

            return root;
        }

        /// <summary>
        /// Sets the value on the underlying instance this field is bound to.
        /// </summary>
        public void SetValueAndNotify(object newValue)
        {
            var oldValue = GetValue();
            valueProvider.SetValue(newValue);

            var changeEvent = ChangeEvent<object>.GetPooled(oldValue, newValue);
            changeEvent.target = this;
            SendEvent(changeEvent);
        }

        public void SetValueWithoutNotify(object newValue)
        {
            valueProvider.SetValue(value);
        }

        /// <summary>
        /// Gets the value on the underlying instance this field is bound to.
        /// </summary>
        public object GetValue()
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

            public int ArrayIndex { get; set; }

            public string ValueName => string.Empty;

            public ListValueProvider(int arrayIndex, IList list)
            {
                this.list  = list;
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
    }

    public class GeneralField<T> : GeneralField
    {
        public GeneralField(IValueProvider valueProvider, Options options = default) : base(typeof(T), valueProvider, options)
        {
        }
    }
}