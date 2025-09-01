using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI.EditorUI
{
    public class NotifyValueChangedGroup<T> : VisualElement, INotifyValueChanged<T>
    {
        public T value { get; set; }

        public void SetValueWithoutNotify(T newValue)
        {
        }

        public NotifyValueChangedGroup(params INotifyValueChanged<T>[] children)
        {
            
        }
    }
}