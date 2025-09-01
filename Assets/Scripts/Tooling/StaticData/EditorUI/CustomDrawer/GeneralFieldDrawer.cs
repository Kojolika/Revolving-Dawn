using System;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI.EditorUI
{
    /// <summary>
    /// Types inheriting this will be instantiated by the <see cref="DrawerManager"/> during the editor when the <see cref="GeneralField"/>
    /// draws the type defined in <see cref="DrawType"/>.
    /// </summary>
    public interface IDrawer
    {
        Type DrawType { get; }
        VisualElement Draw(IValueProvider valueProvider, GeneralField field);
    }

    /// <inheritdoc cref="IDrawer"/>
    public abstract class GeneralFieldDrawer<T> : IDrawer
    {
        Type IDrawer.DrawType => typeof(T);
        public abstract VisualElement Draw(IValueProvider valueProvider, GeneralField field);
    }
}