using System;
using System.Collections;
using System.Collections.Generic;
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
        private FieldInfo fieldInfo;
        private object objectFieldBelongsTo;
        private EventCallback<ChangeEvent<object>> callback;
        private VisualElement visualElementFieldEditor;

        /// <summary>
        /// Index of the array that is being drawn.
        /// </summary>
        private int arrayIndex;

        /// <summary>
        /// Label when this is an array element, null otherwise.
        /// </summary>
        private readonly Label arrayIndexLabel;

        /// <summary>
        /// If <see cref="isArrayElementField"/> true, this is the type of element in the array.
        /// </summary>
        private readonly Type arrayElementType;

        /// <summary>
        /// If GeneralField is drawing an array or list, this is the data source for the elements.
        /// </summary>
        private List<object> itemsSource;

        /// <summary>
        /// Is this field drawing an element of an array.
        /// </summary>
        private readonly bool isArrayElement;

        public GeneralField(FieldInfo fieldInfo, object objectFieldBelongsTo, EventCallback<ChangeEvent<object>> callback = null)
        {
            this.fieldInfo = fieldInfo;
            this.objectFieldBelongsTo = objectFieldBelongsTo;
            this.callback = callback;

            visualElementFieldEditor = DrawEditorForType(fieldInfo.FieldType, callback);
            Add(visualElementFieldEditor);
        }

        /// <summary>
        /// Used internally, when GeneralField draws a list type it will draw GeneralFields as list elements.
        /// </summary>
        private GeneralField(FieldInfo fieldInfo, object objectFieldBelongsTo, int arrayIndex, Type arrayElementType, List<object> itemsSource)
        {
            this.fieldInfo = fieldInfo;
            this.objectFieldBelongsTo = objectFieldBelongsTo;
            this.arrayElementType = arrayElementType;
            this.arrayIndex = arrayIndex;
            this.isArrayElement = true;
            this.itemsSource = itemsSource;

            style.flexDirection = FlexDirection.Row;
            arrayIndexLabel = new Label(arrayIndex.ToString())
            {
                style = { minWidth = 30 }
            };
            Add(arrayIndexLabel);
            Add(DrawEditorForType(arrayElementType));
        }

        private VisualElement DrawEditorForType(Type type, EventCallback<ChangeEvent<object>> callback = null)
        {
            VisualElement editorField;

            if (typeof(int).IsAssignableFrom(type))
            {
                editorField = CreateFieldForType<int, IntegerField>(evt =>
                    callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue))
                );
            }
            else if (typeof(float).IsAssignableFrom(type))
            {
                editorField = CreateFieldForType<float, FloatField>(evt =>
                    callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue))
                );
            }
            else if (typeof(Enum).IsAssignableFrom(type))
            {
                editorField = CreateFieldForType<Enum, EnumField>(evt =>
                    callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue))
                );
            }
            else if (typeof(string).IsAssignableFrom(type))
            {
                editorField = CreateFieldForType<string, TextField>(evt =>
                    callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue))
                );
            }
            else if (typeof(Object).IsAssignableFrom(type))
            {
                editorField = CreateFieldForType<Object, ObjectField>(evt =>
                    callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue))
                );
            }
            else if (typeof(Color).IsAssignableFrom(type))
            {
                editorField = CreateFieldForType<Color, ColorField>(evt =>
                    callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue))
                );
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                editorField = CreateListField(evt =>
                    callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue))
                );
            }
            else
            {
                editorField = new Label($"No editor created for type {type}");
            }

            editorField.style.minWidth = 100;

            return editorField;
        }

        private TField CreateFieldForType<TType, TField>(EventCallback<ChangeEvent<TType>> onValueChanged = null)
            where TField : BaseField<TType>, new()
        {
            var editorField = new TField
            {
                value = (TType)fieldInfo.GetValue(objectFieldBelongsTo)
            };

            editorField.RegisterValueChangedCallback(evt =>
            {
                if (isArrayElement)
                {
                    itemsSource[arrayIndex] = evt.newValue;
                    fieldInfo.SetValue(objectFieldBelongsTo, itemsSource.Select(obj => (TType)obj).ToList());
                }
                else
                {
                    fieldInfo.SetValue(objectFieldBelongsTo, evt.newValue);
                }
                onValueChanged?.Invoke(ChangeEvent<TType>.GetPooled(evt.previousValue, evt.newValue));
            });

            return editorField;
        }

        private VisualElement CreateListField(EventCallback<ChangeEvent<IList>> onValueChanged = null)
        {
            var root = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row }
            };

            itemsSource = ((fieldInfo.GetValue(objectFieldBelongsTo) as IList)
                    ?.Cast<object>() ?? new List<object>())
                .ToList();

            // If the type is an array
            var elementType = fieldInfo.FieldType.IsArray
                ? fieldInfo.FieldType.GetElementType()
                // otherwise if it's a generic list
                : fieldInfo.FieldType.IsGenericType
                    ? fieldInfo.FieldType.GetGenericArguments()[0]
                    : null;

            if (elementType == null)
            {
                MyLogger.LogError($"Field value is an {typeof(IList)} but cannot find element type.");
                return null;
            }

            var listView = new ListView
            {
                itemsSource = itemsSource,
                makeItem = () => new GeneralField(fieldInfo, objectFieldBelongsTo, itemsSource.Count - 1, elementType, itemsSource),
                bindItem = (item, index) =>
                {
                    var generalField = item as GeneralField;
                    generalField!.arrayIndex = index;
                    generalField.arrayIndexLabel.text = generalField.arrayIndex.ToString();
                },
                unbindItem = (item, _) =>
                {
                    var generalField = item as GeneralField;
                    generalField!.arrayIndex = -1;
                    generalField.arrayIndexLabel.text = generalField.arrayIndex.ToString();
                },
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                reorderable = true,
                showBorder = true,
            };

            root.Add(listView);

            var buttonContainer = new VisualElement
            {
                style = { flexDirection = FlexDirection.Column }
            };
            root.Add(buttonContainer);
            buttonContainer.Add(new Button(() =>
            {
                var previousList = itemsSource;
                itemsSource.Add(default);
                onValueChanged?.Invoke(ChangeEvent<IList>.GetPooled(previousList, itemsSource));
                listView.RefreshItems();
            })
            {
                text = "+"
            });

            buttonContainer.Add(new Button(() =>
            {
                var previousList = itemsSource;
                var selectedIndex = listView.selectedIndex;
                selectedIndex = selectedIndex == -1
                    ? itemsSource.Count - 1
                    : selectedIndex;

                if (selectedIndex < 0)
                {
                    return;
                }

                itemsSource.RemoveAt(selectedIndex);
                onValueChanged?.Invoke(ChangeEvent<IList>.GetPooled(previousList, itemsSource));
                listView.RefreshItems();
            })
            {
                text = "-"
            });

            return root;
        }
    }
}