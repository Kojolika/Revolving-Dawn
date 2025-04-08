using System;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    /// <summary>
    /// Types inheriting this will be instantiated by the <see cref="DrawerManager"/> during the editor when the <see cref="GeneralField"/>
    /// draws the type defined in <see cref="DrawType"/>.
    /// </summary>
    public interface IDrawer
    {
        Type DrawType { get; }
        VisualElement Draw(IValueProvider valueProvider);
    }

    /// <inheritdoc cref="IDrawer"/>
    public abstract class GeneralFieldDrawer<T> : IDrawer
    {
        Type IDrawer.DrawType => typeof(T);
        private ValueProvider<T> valueProvider;

        public VisualElement Draw(IValueProvider valueProvider)
        {
            Assert.IsTrue(valueProvider != null);

            if (this.valueProvider != valueProvider)
            {
                this.valueProvider = new ValueProvider<T>(
                    () => (T)valueProvider.GetValue(),
                    value => valueProvider.SetValue(value),
                    valueProvider.ValueName);
            }

            return Draw(this.valueProvider);
        }

        protected abstract VisualElement Draw(ValueProvider<T> valueProvider);
    }
}