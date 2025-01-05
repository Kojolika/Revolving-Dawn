using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Tooling.Logging;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Tooling.StaticData
{
    /// <summary>
    /// A property field that supports multiple types.
    /// </summary>
    public class GeneralField : VisualElement
    {
        private readonly EventCallback<ChangeEvent<object>> callback;

        /// <summary>
        /// Label when this is an array element, null otherwise.
        /// </summary>
        private readonly Label arrayIndexLabel;

        /// <summary>
        /// Fired if the array index for this editor field changes.
        /// </summary>
        private Action<int> onArrayIndexChanged;

        /// <summary>
        /// The object this field is editing.
        /// </summary>
        private readonly object underlyingObject;
        
        /// <summary>
        /// How the value is retrieved from the <see cref="underlyingObject"/>.
        /// </summary>
        private readonly IValueProvider valueProvider;

        private const string StaticDataNullLabel = "null";

        public GeneralField(Type type,
            object underlyingObject,
            IValueProvider valueProvider,
            EventCallback<ChangeEvent<object>> callback = null)
        {
            this.underlyingObject = underlyingObject;
            this.valueProvider = valueProvider;
            this.callback = callback;

            Add(DrawEditorForType(type));
        }

        /// <summary>
        /// Used internally, when GeneralField draws a list type it will draw GeneralFields as list elements.
        /// </summary>
        private GeneralField(Type type,
            IList underlyingObject,
            ArrayValueProvider valueProvider,
            EventCallback<ChangeEvent<object>> callback = null)
        {
            this.underlyingObject = underlyingObject;
            this.valueProvider = valueProvider;
            this.callback = callback;

            style.flexDirection = FlexDirection.Row;
            arrayIndexLabel = new Label(valueProvider.ArrayIndex.ToString())
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
            Add(DrawEditorForType(type));
        }

        /// <summary>
        /// Draws an editor field for the given type.
        /// </summary>
        private VisualElement DrawEditorForType(Type type)
        {
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
            else if (type.GetCustomAttribute<SerializableAttribute>() is not null)
            {
                editorForFieldType = RecursiveDrawElements(type);
            }
            else
            {
                editorForFieldType = new Label($"No editor created for type {type}");
            }

            return editorForFieldType;
        }

        /// <summary>
        /// Creates an editor field for a given type.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <returns></returns>
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
            onArrayIndexChanged += index => { editorForFieldType.value = (TType)((IList)underlyingObject)[index]; };

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
                makeItem = () => new GeneralField(elementType, itemsSource, new ArrayValueProvider(itemsSource.Count - 1)),
                bindItem = (item, index) => ((GeneralField)item).BindArrayIndex(index),
                unbindItem = (item, _) => ((GeneralField)item).BindArrayIndex(-1),
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                reorderable = true,
                showBorder = true,
                style =
                {
                    minWidth = 150
                },
                showBoundCollectionSize = true,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
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

            root.Add(new ButtonIcon(() => InstancesView.Selector.Open(type, OnStaticDataReferenceChanged), IconPaths.List));
            root.Add(nameLabel);

            onArrayIndexChanged += arrayIndex => OnStaticDataReferenceChanged(((IList)underlyingObject)[arrayIndex] as StaticData);

            return root;

            // local function
            void OnStaticDataReferenceChanged(StaticData staticData)
            {
                SetValue(staticData);
                nameLabel.text = (GetValue() as StaticData)?.Name ?? StaticDataNullLabel;
            }
        }

        // TODO: List support
        /// <summary>
        /// Similar to how the inspector draws serializable types. Draws all fields of the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private VisualElement RecursiveDrawElements(Type type)
        {
            var root = new VisualElement();
            foreach (var field in type.GetFields(EditorWindow.BindingFlagsToSelectStaticDataFields))
            {
                //root.Add(new GeneralField(field, objectFieldBelongsTo, callback));
            }

            return root;
        }

        private VisualElement CreateAbstractTypeSelection(Type type)
        {
            var root = new VisualElement();
            var concreteTypes = type.Assembly.DefinedTypes
                .Where(t => type.IsAssignableFrom(t)
                            && !t.IsAbstract
                            && !t.IsInterface)
                .Select(t => t.ToString())
                .ToList();

            if (concreteTypes.Count < 1)
            {
                return new Label($"No types inherit from {type}");
            }

            var dropDown = new DropdownField(concreteTypes, concreteTypes[0])
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    marginLeft = 0
                }
            };
            root.Add(dropDown);

            var interfaceField = new GeneralField(Type.GetType(concreteTypes[0]), underlyingObject, valueProvider);
            root.Add(interfaceField);
            dropDown.RegisterValueChangedCallback(evt =>
            {
                var dropDownType = Type.GetType(evt.newValue);
                if (dropDownType == null)
                {
                    MyLogger.LogError($"Selected type {evt.newValue} but cannot find type in assembly!");
                    return;
                }

                root.Remove(interfaceField);
                interfaceField = new GeneralField(dropDownType, underlyingObject, valueProvider);
                root.Add(interfaceField);
            });


            // Set initialValue
            //SetValue(Activator.CreateInstance(Type.GetType(concreteTypes[0])!));

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
            onArrayIndexChanged?.Invoke(index);
        }

        /// <summary>
        /// Sets the value on the underlying instance this field is bound to.
        /// The <see cref="underlyingObject"/>.
        /// </summary>
        private void SetValue(object value)
        {
            var prevValue = valueProvider.GetValue(underlyingObject);
            valueProvider.SetValue(underlyingObject, value);
            callback?.Invoke(ChangeEvent<object>.GetPooled(prevValue, value));
        }

        /// <summary>
        /// Gets the value on the underlying instance this field is bound to.
        /// The <see cref="underlyingObject"/>.
        /// </summary>
        private object GetValue()
        {
            return valueProvider.GetValue(underlyingObject);
        }
    }
}