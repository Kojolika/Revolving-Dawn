using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Tooling.StaticData
{
    public class EnumerableField<T> : BaseField<IEnumerable<T>>
    {
        public EnumerableField(string label, VisualElement visualInput) : base(label, visualInput)
        {
            visualInput.Add(new Label("test"));
        }
    }
}