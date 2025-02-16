using System;
using System.Collections.Generic;
using System.Linq;
using Tooling.Logging;


namespace Tooling.StaticData.EditorUI
{
    /// <summary>
    /// Classes in this project that inherit from <see cref="IDecorator"/> that have an open (parameterless) constructor
    /// will automatically have their <see cref="IDecorator.DecorateElement"/> function called
    /// on the <see cref="IDecorator.DecorateElementType"/> that is drawn by the <see cref="GeneralField"/>
    /// </summary>
    public class DecoratorManager
    {
        private static DecoratorManager instance;

        public static DecoratorManager Instance => instance ??= new DecoratorManager
        {
            Decorators = BuildCallbackDictionary()
        };

        public Dictionary<Type, IDecorator> Decorators { get; private set; } = new();

        private DecoratorManager()
        {
        }

        private static Dictionary<Type, IDecorator> BuildCallbackDictionary()
        {
            var dictionary = new Dictionary<Type, IDecorator>();

            var decoratorTypes = typeof(DecoratorManager).Assembly.DefinedTypes
                .Where(type => typeof(IDecorator).IsAssignableFrom(type)
                               && !type.IsAbstract
                               && !type.IsInterface
                               && type.GetConstructor(Type.EmptyTypes) != null);

            foreach (var type in decoratorTypes)
            {
                var decorator = Activator.CreateInstance(type) as IDecorator;
                if (decorator == null)
                {
                    MyLogger.LogWarning($"Invalid callback type for {type}");
                    continue;
                }

                var typeReceivingCallback = decorator.DecorateElementType;
                if (!dictionary.TryAdd(typeReceivingCallback, decorator))
                {
                    MyLogger.LogWarning($"There's already a decorator type {typeReceivingCallback} defined in out decorator dictionary." +
                                        $"Ignoring this decorator from type {type}");
                }
            }

            return dictionary;
        }
    }
}