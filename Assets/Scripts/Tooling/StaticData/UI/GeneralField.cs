using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tooling.Logging;
using UnityEditor;
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
        private readonly FieldInfo fieldInfo;
        private readonly object objectFieldBelongsTo;
        private readonly EventCallback<ChangeEvent<object>> callback;

        /// <summary>
        /// Index of the array that is being drawn.
        /// </summary>
        private int arrayIndex;

        /// <summary>
        /// Label when this is an array element, null otherwise.
        /// </summary>
        private readonly Label arrayIndexLabel;

        /// <summary>
        /// If GeneralField is drawing an array or list, this is the data source for the elements.
        /// </summary>
        private IList itemsSource;

        /// <summary>
        /// Is this field drawing an element of an array.
        /// </summary>
        private readonly bool isArrayElement;

        /// <summary>
        /// Fired if the array index for this editor field changes.
        /// Only used if <see cref="isArrayElement"/> is true.
        /// </summary>
        private Action<int> onArrayIndexChanged;

        public GeneralField(FieldInfo fieldInfo, object objectFieldBelongsTo, EventCallback<ChangeEvent<object>> callback = null)
        {
            this.fieldInfo = fieldInfo;
            this.objectFieldBelongsTo = objectFieldBelongsTo;
            this.callback = callback;

            Add(DrawEditorForType(fieldInfo.FieldType));
        }

        /// <summary>
        /// Used internally, when GeneralField draws a list type it will draw GeneralFields as list elements.
        /// </summary>
        private GeneralField(FieldInfo fieldInfo,
            object objectFieldBelongsTo,
            int arrayIndex,
            Type arrayElementType,
            IList itemsSource)
        {
            this.fieldInfo = fieldInfo;
            this.objectFieldBelongsTo = objectFieldBelongsTo;
            this.arrayIndex = arrayIndex;
            this.isArrayElement = true;
            this.itemsSource = itemsSource;

            style.flexDirection = FlexDirection.Row;
            arrayIndexLabel = new Label(arrayIndex.ToString())
            {
                style = { minWidth = 40, alignSelf = Align.Center }
            };
            Add(arrayIndexLabel);
            Add(DrawEditorForType(arrayElementType));
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
                editorForFieldType = CreateListField();
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
                value = isArrayElement
                    ? (TType)(fieldInfo.GetValue(objectFieldBelongsTo) as IList)![arrayIndex]
                    : (TType)fieldInfo.GetValue(objectFieldBelongsTo),
                style =
                {
                    marginLeft = 0,
                    minWidth = 150
                }
            };

            editorForFieldType.RegisterValueChangedCallback(evt => SetValue(evt.newValue));
            onArrayIndexChanged += index => { editorForFieldType.value = (TType)itemsSource[index]; };

            return editorForFieldType;
        }

        /// <summary>
        /// Creates a custom list view field that draws GeneralFields as its listview elements
        /// </summary>
        /// <returns>A visual element containing the list view.</returns>
        /// <exception cref="ArgumentException">Fired if the fieldInfo is not a type of <see cref="IList"/></exception>
        private VisualElement CreateListField()
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
            var elementType = fieldInfo.FieldType.IsArray
                ? fieldInfo.FieldType.GetElementType()
                // otherwise if it's a generic list
                : fieldInfo.FieldType.IsGenericType
                    ? fieldInfo.FieldType.GetGenericArguments()[0]
                    : throw new ArgumentException($"Field value is an {typeof(IList)} but cannot find element type.");

            itemsSource = fieldInfo.GetValue(objectFieldBelongsTo) as IList
                          ?? Activator.CreateInstance(fieldInfo.FieldType) as IList;

            var listView = new ListView
            {
                itemsSource = itemsSource,
                makeItem = () => new GeneralField(fieldInfo, objectFieldBelongsTo, itemsSource.Count - 1, elementType, itemsSource),
                bindItem = (item, index) =>
                {
                    var generalField = item as GeneralField;
                    generalField!.BindArrayIndex(index);
                    generalField.arrayIndexLabel.text = generalField.arrayIndex.ToString();
                },
                unbindItem = (item, _) =>
                {
                    var generalField = item as GeneralField;
                    generalField!.BindArrayIndex(-1);
                    generalField.arrayIndexLabel.text = generalField.arrayIndex.ToString();
                },
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                reorderable = true,
                showBorder = true,
                style =
                {
                    minWidth = 150
                }
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

            var nameLabel = new Label((fieldInfo.GetValue(objectFieldBelongsTo) as StaticData)?.Name ?? "None")
            {
                style = { alignSelf = Align.Center, minWidth = 40 }
            };

            root.Add(new ButtonIcon(() => InstancesView.Selector.Open(type, OnStaticDataReferenceChanged), IconPaths.List));
            root.Add(nameLabel);

            onArrayIndexChanged += arrayIndex => OnStaticDataReferenceChanged(itemsSource[arrayIndex] as StaticData);

            return root;

            // local function
            void OnStaticDataReferenceChanged(StaticData staticData)
            {
                SetValue(staticData);
                nameLabel.text = GetValue<StaticData>()?.Name ?? "None";
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
                root.Add(new GeneralField(field, objectFieldBelongsTo, callback));
            }

            return root;
        }

        private VisualElement CreateAbstractTypeSelection(Type type)
        {
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

            var dropDown = new DropdownField(concreteTypes, concreteTypes[0]);
            dropDown.RegisterValueChangedCallback(evt =>
            {
                var dropDownType = Type.GetType(evt.newValue);
                if (dropDownType == null)
                {
                    MyLogger.LogError($"Selected type {evt.newValue} but cannot find type in assembly!");
                    return;
                }

                SetValue(Activator.CreateInstance(dropDownType));
            });

            // Set initialValue
            SetValue(Activator.CreateInstance(Type.GetType(concreteTypes[0])!));

            return dropDown;
        }

        /// <summary>
        /// Only applies if this is drawing an element in a list. Notifies the editor field that index
        /// is where the value should get retrieved from in <see cref="itemsSource"/>
        /// </summary>
        /// <param name="index"></param>
        private void BindArrayIndex(int index)
        {
            // index is set to -1 when the element is not bound
            if (!isArrayElement || index < 0 || index >= itemsSource?.Count)
            {
                return;
            }

            arrayIndex = index;
            onArrayIndexChanged?.Invoke(index);
        }


        /// <summary>
        /// Sets the value on the underlying instance this field is bound to.
        /// The <see cref="objectFieldBelongsTo"/>.
        /// </summary>
        private void SetValue<T>(T value)
        {
            if (isArrayElement)
            {
                var prevList = fieldInfo.GetValue(objectFieldBelongsTo) as IList;
                var newList = prevList;
                newList![arrayIndex] = value;
                fieldInfo.SetValue(objectFieldBelongsTo, newList);
                callback?.Invoke(ChangeEvent<object>.GetPooled(prevList[arrayIndex], newList[arrayIndex]));
            }
            else
            {
                var prevValue = (T)fieldInfo.GetValue(objectFieldBelongsTo);
                fieldInfo.SetValue(objectFieldBelongsTo, value);
                callback?.Invoke(ChangeEvent<object>.GetPooled(prevValue, value));
            }
        }

        /// <summary>
        /// Gets the value on the underlying instance this field is bound to.
        /// The <see cref="objectFieldBelongsTo"/>.
        /// </summary>
        private T GetValue<T>()
        {
            return isArrayElement
                ? (T)(fieldInfo.GetValue(objectFieldBelongsTo) as IList)?[arrayIndex]
                : (T)fieldInfo.GetValue(objectFieldBelongsTo);
        }
    }
}