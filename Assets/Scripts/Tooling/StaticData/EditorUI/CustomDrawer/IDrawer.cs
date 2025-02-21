using System;
using Tooling.Logging;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    public interface IDrawer
    {
        Type DrawType { get; }
        VisualElement Draw(Func<object> getValueFunc, Action<object> setValueFunc);
    }

    public interface IDrawer<T> : IDrawer
    {
        Type IDrawer.DrawType => typeof(T);

        VisualElement IDrawer.Draw(Func<object> getValueFunc, Action<object> setValueFunc)
        {
            return Draw(() => (T)(getValueFunc?.Invoke() ?? default(T)), obj => setValueFunc.Invoke(obj));
        }

        VisualElement Draw(Func<T> getValueFunc, Action<T> setValueFunc);
    }
}