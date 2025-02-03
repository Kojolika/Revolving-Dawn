using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Fight.Engine.Bytecode;
using ModestTree;
using Tooling.Logging;
using Tooling.StaticData.Attributes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Tooling.StaticData
{
    /// <summary>
    /// A property field that supports multiple types.
    /// </summary>
    public class GeneralField : VisualElement
    {
        private readonly Type type;

        /// <summary>
        /// The object this field is editing.
        /// </summary>
        private readonly object underlyingObject;

        /// <summary>
        /// How the value is retrieved from the <see cref="underlyingObject"/>.
        /// </summary>
        private readonly IValueProvider valueProvider;

        private EventCallback<ChangeEvent<object>> callback;

        /// <summary>
        /// Label when this is an array element, null otherwise.
        /// </summary>
        private Label arrayIndexLabel;

        private readonly bool isArrayElementField;

        private const string StaticDataNullLabel = "null";

        public GeneralField(Type type,
            object underlyingObject,
            IValueProvider valueProvider,
            EventCallback<ChangeEvent<object>> callback = null,
            bool isArrayElementField = false)
        {
            this.type = type;
            this.underlyingObject = underlyingObject;
            this.valueProvider = valueProvider;
            this.callback = callback;
            this.isArrayElementField = isArrayElementField;

            RefreshView();
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

            VisualElement editorForFieldType;

            if (typeof(int).IsAssignableFrom(type))
            {
                editorForFieldType = CreateFieldForType<int, IntegerField>();
            }
            else if (typeof(long).IsAssignableFrom(type))
            {
                editorForFieldType = CreateFieldForType<long, LongField>();
            }
            else if (typeof(float).IsAssignableFrom(type))
            {
                editorForFieldType = CreateFieldForType<float, FloatField>();
            }
            else if (typeof(bool).IsAssignableFrom(type))
            {
                editorForFieldType = CreateFieldForType<bool, Toggle>();
            }
            else if (typeof(Enum).IsAssignableFrom(type))
            {
                editorForFieldType = CreateFieldForType<Enum, EnumField>();
            }
            else if (typeof(string).IsAssignableFrom(type))
            {
                editorForFieldType = CreateFieldForType<string, TextField>();
            }
            else if (typeof(Object).IsAssignableFrom(type))
            {
                editorForFieldType = CreateFieldForType<Object, ObjectField>();
                ((ObjectField)editorForFieldType).objectType = type;
            }
            else if (typeof(Color).IsAssignableFrom(type))
            {
                editorForFieldType = CreateFieldForType<Color, ColorField>();
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                editorForFieldType = CreateListField(type);
            }
            else if (typeof(StaticData).IsAssignableFrom(type))
            {
                editorForFieldType = CreateStaticDataField(type);
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

            return editorForFieldType;
        }

        /// <summary>
        /// Creates an editor field for a given type.
        /// </summary>
        private TField CreateFieldForType<TType, TField>()
            where TField : BaseField<TType>, new()
        {
            var editorForFieldType = new TField
            {
                value = (TType)GetValue(),
                style =
                {
                    marginLeft = 0,
                    minWidth = 150
                }
            };

            editorForFieldType.RegisterValueChangedCallback(evt => SetValue(evt.newValue));

            return editorForFieldType;
        }

        /// <summary>
        /// Creates a custom list view field that draws GeneralFields as its listview elements
        /// </summary>
        /// <returns>A visual element containing the list view.</returns>
        /// <exception cref="ArgumentException">Fired if the fieldInfo is not a type of <see cref="IList"/></exception>
        private VisualElement CreateListField(Type type)
        {
            var root = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    minWidth = 200
                }
            };
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
                makeItem = () => new GeneralField(elementType, itemsSource, new ArrayValueProvider(itemsSource.Count - 1), null, true),
                bindItem = (item, index) => ((GeneralField)item).BindArrayIndex(index),
                unbindItem = (item, _) => ((GeneralField)item).BindArrayIndex(-1),
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                reorderable = true,
                showBorder = true,
                style = { minWidth = 150 },
                showBoundCollectionSize = true,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderMode = ListViewReorderMode.Animated,
                showFoldoutHeader = true
            };

            root.Add(listView);

            var buttonContainer = new VisualElement
            {
                style = { flexDirection = FlexDirection.Column }
            };
            root.Add(buttonContainer);
            buttonContainer.Add(new Button(() =>
            {
                itemsSource.Add(default);
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

        private VisualElement CreateStaticDataField(Type type)
        {
            var root = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.FlexStart
                }
            };

            var nameLabel = new Label((GetValue() as StaticData)?.Name ?? StaticDataNullLabel)
            {
                style =
                {
                    alignSelf = Align.Center, minWidth = 40
                }
            };

            root.Add(new ButtonIcon(
                () => InstancesView.Selector.Open(type, staticData => OnStaticDataReferenceChanged(staticData, true)),
                IconPaths.List)
            );
            root.Add(nameLabel);

            callback += evt => { OnStaticDataReferenceChanged(evt.newValue as StaticData, false); };

            return root;

            // local function
            void OnStaticDataReferenceChanged(StaticData staticData, bool notify)
            {
                SetValue(staticData, notify);
                nameLabel.text = (GetValue() as StaticData)?.Name ?? StaticDataNullLabel;
            }
        }

        /// <summary>
        /// Similar to how the inspector draws serializable types. Draws all serializable fields of the given type.
        /// </summary>
        private VisualElement RecursiveDrawElements(Type type)
        {
            var currentObj = GetValue();
            if (currentObj == null)
            {
                currentObj = Activator.CreateInstance(type);
                SetValue(currentObj);
            }

            var root = new VisualElement();
            foreach (var field in Utils.GetFields(type))
            {
                root.Add(
                    new GeneralField(
                        field.FieldType,
                        currentObj,
                        new FieldValueProvider(field),
                        callback
                    )
                );
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
                            && t.GetCustomAttribute<GeneralFieldIgnoreAttribute>() == null
                            && t.GetInterfaces()
                                .All(tInterface => tInterface.GetCustomAttribute<GeneralFieldIgnoreAttribute>() == null))
                .ToList();

            var concreteTypesAsStrings = concreteTypes
                .Select(t => t.ToString())
                .ToList();

            if (concreteTypes.Count < 1)
            {
                return new Label($"No types inherit from {type}");
            }

            var currentValueType = GetValue()?.GetType();
            var typeToDisplay = concreteTypes.Contains(currentValueType)
                ? currentValueType
                : concreteTypes.First();

            var dropDown = new DropdownField(concreteTypesAsStrings, typeToDisplay?.ToString())
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    marginLeft = 0
                }
            };

            root.Add(dropDown);

            var interfaceField = CreateGeneralFieldForInterfaceType(typeToDisplay);
            root.Add(interfaceField);

            dropDown.RegisterValueChangedCallback(evt =>
            {
                typeToDisplay = Type.GetType(evt.newValue);
                if (typeToDisplay == null)
                {
                    MyLogger.LogError($"Selected type {evt.newValue} but cannot find type in assembly!");
                    return;
                }

                OnTypeSelected(typeToDisplay, Type.GetType(evt.previousValue));

                root.Remove(interfaceField);
                interfaceField = CreateGeneralFieldForInterfaceType(typeToDisplay);
                root.Add(interfaceField);
            });

            OnTypeSelected(typeToDisplay, currentValueType);

            return root;

            GeneralField CreateGeneralFieldForInterfaceType(Type newType)
            {
                return new GeneralField(newType, underlyingObject, valueProvider);
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
            var root = new VisualElement();

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
        /// Only applies if this is drawing an element in a list. Notifies the editor field that index
        /// is where the value should get retrieved from in <see cref="underlyingObject"/>
        /// </summary>
        /// <param name="index"></param>
        private void BindArrayIndex(int index)
        {
            if (index < 0 || index >= ((IList)underlyingObject)?.Count)
            {
                return;
            }

            ((ArrayValueProvider)valueProvider).ArrayIndex = index;
            arrayIndexLabel.text = index.ToString();

            RefreshView();
        }

        private void RefreshView()
        {
            Clear();

            if (isArrayElementField)
            {
                style.flexDirection = FlexDirection.Row;
                arrayIndexLabel = new Label(((ArrayValueProvider)valueProvider).ArrayIndex.ToString())
                {
                    style =
                    {
                        minWidth = 16,
                        alignSelf = Align.Center,
                        alignItems = Align.Center,
                        alignContent = Align.Center,
                        unityTextAlign = TextAnchor.MiddleCenter,
                    }
                };
                Add(arrayIndexLabel);
            }

            Add(DrawEditorForType(type));
        }

        /// <summary>
        /// Sets the value on the underlying instance this field is bound to.
        /// The <see cref="underlyingObject"/>.
        /// </summary>
        private void SetValue(object value, bool notify = true)
        {
            var prevValue = valueProvider.GetValue(underlyingObject);
            valueProvider.SetValue(underlyingObject, value);

            if (notify)
            {
                callback?.Invoke(ChangeEvent<object>.GetPooled(prevValue, value));
            }
        }

        /// <summary>
        /// Gets the value on the underlying instance this field is bound to.
        /// The <see cref="underlyingObject"/>.
        /// </summary>
        private object GetValue()
        {
            return valueProvider.GetValue(underlyingObject);
        }

        /// <summary>
        /// Returns what to draw when the specified type cannot be drawn.
        /// </summary>
        private VisualElement GetErrorElement(Type type)
        {
            return new Label($"No editor created for type {type}");
        }
    }
}