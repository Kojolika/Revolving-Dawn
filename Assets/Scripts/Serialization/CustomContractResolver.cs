using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tooling.Logging;
using Tooling.StaticData;
using Zenject;

namespace Serialization
{
    /// <summary>
    /// This class is used to inject dependencies into classes which are deserialized form JSON.
    /// </summary>
    public class CustomContractResolver : DefaultContractResolver
    {
        private readonly DiContainer diContainer;
        private readonly NamingStrategy namingStrategy = new DefaultNamingStrategy();

        /// <summary>
        /// Used during runtime to instantiate objects with our di container.
        /// </summary>
        public CustomContractResolver(DiContainer diContainer)
        {
            this.diContainer = diContainer;
            NamingStrategy = namingStrategy;
        }

        public CustomContractResolver()
        {
            NamingStrategy = namingStrategy;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var objectContract = base.CreateObjectContract(objectType);

            // Instantiate objects that are bound to our container with its dependencies
            if (diContainer?.HasBinding(objectType) ?? false)
            {
                // Override objection creation here
                objectContract.OverrideCreator = args => diContainer.Instantiate(objectType, args);
            }

            return objectContract;
        }
    }
}