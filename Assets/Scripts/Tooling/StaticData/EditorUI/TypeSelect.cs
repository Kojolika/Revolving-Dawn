using System;
using System.Collections.Generic;
using System.Linq;
using Tooling.Logging;
using UnityEngine.UIElements;

/// <summary>
/// A dropdown for selecting a derived type of <see cref="T"/>.
/// Handles instantiating the type and caches previous selections.
/// </summary>
/// <typeparam name="T">Type to select a derived type from</typeparam>
public class TypeSelect<T> : VisualElement, INotifyValueChanged<T>
{
    public T value { get; set; }
    private readonly string label;
    private readonly Func<Type, T> constructor;
    private readonly Dictionary<Type, T> objectCache = new();

    public void SetValueWithoutNotify(T newValue)
    {
        value = newValue;
    }

    public TypeSelect(string label, T value, Func<Type, T> constructor = null)
    {
        this.label = label;
        this.value = value;

        // Default the constructor to a parameterless ctor
        constructor ??= type => (T)Activator.CreateInstance(type);
        this.constructor = constructor;
        RefreshView();
    }

    private void RefreshView()
    {
        Clear();

        var type = typeof(T);
        var concreteTypes = type.Assembly.DefinedTypes
            .Where(t => type.IsAssignableFrom(t)
                        && !t.IsAbstract
                        && !t.IsInterface)
            .Select(tInfo => tInfo.AsType())
            .ToList();

        if (concreteTypes.Count < 1)
        {
            Add(new Label($"No types inherit from {type}"));
            return;
        }

        var currentValueType = value?.GetType();
        var typeToDisplay = concreteTypes.Contains(currentValueType)
            ? currentValueType
            : concreteTypes.First();

        var popupField = new PopupField<Type>(
            label,
            concreteTypes,
            typeToDisplay,
            t => $"{t.Name} {(!string.IsNullOrEmpty(t.Namespace) ? $" ({t.Namespace})" : string.Empty)}",
            t => $"{t.Name} {(!string.IsNullOrEmpty(t.Namespace) ? $" ({t.Namespace})" : string.Empty)}");

        Add(popupField);

        var typeInfoContainer = new VisualElement();

        popupField.RegisterValueChangedCallback(evt =>
        {
            typeInfoContainer.Clear();

            typeToDisplay = evt.newValue;
            if (typeToDisplay == null)
            {
                MyLogger.LogError($"Selected type {evt.newValue} but cannot find type in assembly!");
                return;
            }

            if (value != null && value.GetType() == typeToDisplay)
            {
                return;
            }

            if (!objectCache.TryGetValue(typeToDisplay, out var cachedObject))
            {
                cachedObject = constructor.Invoke(typeToDisplay);
                objectCache.Add(typeToDisplay, cachedObject);
            }

            var changeEvent = ChangeEvent<T>.GetPooled(value, cachedObject);
            changeEvent.target = this;
            SendEvent(changeEvent);

            value = cachedObject;
        });
    }
}