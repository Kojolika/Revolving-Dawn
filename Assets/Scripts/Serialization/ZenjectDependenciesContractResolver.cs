using System;
using Models.Map;
using Newtonsoft.Json.Serialization;
using Zenject;

namespace Serialization
{
    /// <summary>
    /// This class is used to inject dependencies into classes which are deserialized form JSON.
    /// </summary>
    public class ZenjectDependenciesContractResolver : DefaultContractResolver
    {
        private readonly IInstantiator instantiator;
        public ZenjectDependenciesContractResolver(IInstantiator instantiator)
        {
            this.instantiator = instantiator;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var objectContract = base.CreateObjectContract(objectType);

            // We can check for more types here if the object hierarchy would have several.
            // Default handling is used if the type doesn't match the ones we set the OverrideCreator
            // property for.
            if (typeof(NodeEvent).IsAssignableFrom(objectType))
            {
                // Override objection creation here
                objectContract.OverrideCreator = args => instantiator.Instantiate(objectType, args);
            }

            return objectContract;
        }
    }
}