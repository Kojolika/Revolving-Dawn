using System;
using System.Collections.Generic;
using System.Reflection;
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
        public GeneralField(FieldInfo fieldInfo, object objectFieldBelongsTo, EventCallback<ChangeEvent<object>> callback = null)
        {
            Add(DrawEditorForFieldType(fieldInfo, objectFieldBelongsTo, callback));
        }

        private VisualElement DrawEditorForFieldType(FieldInfo field, 
            object objectFieldBelongsTo,
            EventCallback<ChangeEvent<object>> callback = null)
        {
            var fieldType = field.FieldType;
            // kind of hacky but no way to do a switch statement on types
            var supportedTypesDict = new Dictionary<Type, VisualElement>
            {
                {
                    typeof(int),
                    CreateIntField(field,
                        objectFieldBelongsTo,
                        evt => { callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue)); })
                },
                {
                    typeof(float),
                    CreateFloatField(field,
                        objectFieldBelongsTo,
                        evt => { callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue)); })
                },
                {
                    typeof(Enum),
                    CreateEnumField(field,
                        objectFieldBelongsTo,
                        evt => { callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue)); })
                },
                {
                    typeof(string),
                    CreateTextField(field,
                        objectFieldBelongsTo,
                        evt => { callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue)); })
                },
                {
                    typeof(Object),
                    CreateObjectField(field,
                        objectFieldBelongsTo,
                        evt => { callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue)); })
                },
                {
                    typeof(Color),
                    CreateColorField(field,
                        objectFieldBelongsTo,
                        evt => { callback?.Invoke(ChangeEvent<object>.GetPooled(evt.previousValue, evt.newValue)); })
                }
            };

            if (supportedTypesDict.TryGetValue(fieldType, out var visualElement))
            {
                visualElement.style.minWidth = 100;
                return visualElement;
            }

            return new Label($"No editor created for type {fieldType}");
        }

        // a little tedious but we need to enumerate each type and its callbacks
        private IntegerField CreateIntField(FieldInfo field,
            object objectFieldBelongsTo,
            EventCallback<ChangeEvent<int>> onValueChanged = null)
        {
            var intField = new IntegerField();
            intField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(objectFieldBelongsTo, evt.newValue);
                onValueChanged?.Invoke(ChangeEvent<int>.GetPooled(evt.previousValue, evt.newValue));
            });
            return intField;
        }

        private FloatField CreateFloatField(FieldInfo field,
            object objectFieldBelongsTo,
            EventCallback<ChangeEvent<float>> onValueChanged = null)
        {
            var floatField = new FloatField();
            floatField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(objectFieldBelongsTo, evt.newValue);
                onValueChanged?.Invoke(ChangeEvent<float>.GetPooled(evt.previousValue, evt.newValue));
            });
            return floatField;
        }

        private EnumField CreateEnumField(FieldInfo field,
            object objectFieldBelongsTo,
            EventCallback<ChangeEvent<Enum>> onValueChanged = null)
        {
            var enumField = new EnumField();
            enumField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(objectFieldBelongsTo, evt.newValue);
                onValueChanged?.Invoke(ChangeEvent<Enum>.GetPooled(evt.previousValue, evt.newValue));
            });
            return enumField;
        }

        private ObjectField CreateObjectField(FieldInfo field,
            object objectFieldBelongsTo,
            EventCallback<ChangeEvent<Object>> onValueChanged = null)
        {
            var objectField = new ObjectField();
            objectField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(objectFieldBelongsTo, evt.newValue);
                onValueChanged?.Invoke(ChangeEvent<Object>.GetPooled(evt.previousValue, evt.newValue));
            });
            return objectField;
        }

        private ColorField CreateColorField(FieldInfo field,
            object objectFieldBelongsTo,
            EventCallback<ChangeEvent<Color>> onValueChanged = null)
        {
            var colorField = new ColorField();
            colorField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(objectFieldBelongsTo, evt.newValue);
                onValueChanged?.Invoke(ChangeEvent<Color>.GetPooled(evt.previousValue, evt.newValue));
            });
            return colorField;
        }

        private TextField CreateTextField(FieldInfo field,
            object objectFieldBelongsTo,
            EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            var textField = new TextField();
            textField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(objectFieldBelongsTo, evt.newValue);
                onValueChanged?.Invoke(ChangeEvent<string>.GetPooled(evt.previousValue, evt.newValue));
            });
            return textField;
        }
    }
}