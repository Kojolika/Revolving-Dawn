using System;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    public interface ICustomStaticDataDrawer
    {
        Type DrawType { get; }
        event Action OnValueChanged;
        public VisualElement Draw(StaticData data);
    }

    public abstract class CustomStaticDataDrawer<T> : ICustomStaticDataDrawer where T : StaticData
    {
        public Type DrawType => typeof(T);
        public event Action OnValueChanged;

        protected void InvokeValueChanged()
        {
            OnValueChanged?.Invoke();
        }

        public VisualElement Draw(StaticData data)
        {
            return Draw((T)data);
        }

        protected abstract VisualElement Draw(T data);
    }
}