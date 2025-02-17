using System;
using System.Collections.Generic;
using System.Linq;
using Tooling.Logging;

namespace Tooling.StaticData.EditorUI.CustomDrawer
{
    /// <summary>
    /// Classes in this project that inherit from <see cref="IDrawer"/> that have an open (parameterless) constructor
    /// will automatically have their <see cref="IDrawer.Draw"/> function called
    /// on the <see cref="IDrawer.DrawType"/> that is drawn by the <see cref="GeneralField"/>
    /// </summary>
    public class DrawerManager
    {
        private static DrawerManager instance;

        public static DrawerManager Instance => instance ??= new DrawerManager
        {
            Drawers = BuildDrawerDictionary()
        };

        public Dictionary<Type, IDrawer> Drawers { get; private set; } = new();

        private DrawerManager()
        {
        }

        private static Dictionary<Type, IDrawer> BuildDrawerDictionary()
        {
            var dictionary = new Dictionary<Type, IDrawer>();

            var decoratorTypes = typeof(DecoratorManager).Assembly.DefinedTypes
                .Where(type => typeof(IDrawer).IsAssignableFrom(type)
                               && !type.IsAbstract
                               && !type.IsInterface
                               && type.GetConstructor(Type.EmptyTypes) != null);

            foreach (var type in decoratorTypes)
            {
                var decorator = Activator.CreateInstance(type) as IDrawer;
                if (decorator == null)
                {
                    MyLogger.LogWarning($"Invalid callback type for {type}");
                    continue;
                }

                var typeReceivingCallback = decorator.DrawType;
                if (!dictionary.TryAdd(typeReceivingCallback, decorator))
                {
                    MyLogger.LogWarning($"There's already a {nameof(IDrawer)} type {typeReceivingCallback} defined in our {nameof(IDrawer)} dictionary." +
                                        $"Ignoring this {nameof(IDrawer)} from type {type}");
                }
            }

            return dictionary;
        }
    }
}