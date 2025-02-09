using System;

namespace Tooling.StaticData.EditorUI
{
    public interface IDecorator<in T> : IDecorator
    {
        /// <summary>
        /// Implement this to draw additional visual elements around the type <see cref="T"/> when the <see cref="GeneralField"/>
        /// draws this type
        /// </summary>
        public void DecorateElement(GeneralField generalField, T element);

        Type IDecorator.DecorateElementType => typeof(T);

        void IDecorator.DecorateElement(GeneralField generalField, object element)
        {
            DecorateElement(generalField, (T)element);
        }
    }

    // TODO: Won't work for static data types with how its built, do we need to change it?
    // Can pass in a decorator on ctor of General Field like:
    // this.decorator = decorator ?? DecoratorManager.Instance.Decorators.GetValueOrDefault(type);
    public interface IDecorator
    {
        /// <summary>
        /// The type the <see cref="GeneralField"/> is drawing.
        /// </summary>
        Type DecorateElementType { get; }

        /// <summary>
        /// Allows custom control of how the <see cref="GeneralField"/> draws the <see cref="DecorateElementType"/> type.
        /// </summary>
        void DecorateElement(GeneralField generalField, object element);
    }
}