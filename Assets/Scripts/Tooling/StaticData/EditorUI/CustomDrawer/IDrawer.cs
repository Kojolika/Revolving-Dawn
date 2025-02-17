using System;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI.CustomDrawer
{
    public interface IDrawer
    {
        Type DrawType { get; }
        IValueProvider ValueProvider { get; set; }
        VisualElement Draw(object data);
    }

    public interface IDrawer<in T> : IDrawer
    {
        Type IDrawer.DrawType => typeof(T);
        VisualElement Draw(T data);

        VisualElement IDrawer.Draw(object data)
        {
            return Draw((T)data);
        }
    }
}