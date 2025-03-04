using System;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    /// <summary>
    /// Types inheriting this will be instantiated by the <see cref="DrawerManager"/> during the editor when the <see cref="GeneralField"/>
    /// draws the type defined in <see cref="DrawType"/>.
    /// <seealso cref="DrawerManager"/>, <seealso cref="GeneralField"/>
    /// </summary>
    public interface IDrawer
    {
        Type DrawType { get; }
        VisualElement Draw(Func<object> getValueFunc, Action<object> setValueFunc, string label);
    }

    /// <inheritdoc cref="IDrawer"/>
    public interface IDrawer<T> : IDrawer
    {
        Type IDrawer.DrawType => typeof(T);

        VisualElement IDrawer.Draw(Func<object> getValueFunc, Action<object> setValueFunc, string label)
        {
            return Draw(() => (T)(getValueFunc?.Invoke() ?? default(T)), obj => setValueFunc.Invoke(obj), label);
        }

        VisualElement Draw(Func<T> getValueFunc, Action<T> setValueFunc, string label);
    }
}