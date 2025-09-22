using System;
using Newtonsoft.Json;
using Tooling.StaticData.Validation;

namespace Tooling.StaticData.Data
{
    public abstract class StaticData
    {
        // we're using the name as file names, no need to serialize again
        [JsonIgnore, Required(allowDefaultValues: false), Unique]
        public string Name;

        public StaticDataReference Reference { get; set; }
    }

    /// <summary>
    /// Used to serialize a reference to a <see cref="StaticData"/> so we can load it at runtime.
    /// </summary>
    public class StaticDataReference
    {
        /// <summary>
        /// The type of <see cref="StaticData"/> that we're referencing.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// The instance name of <see cref="StaticData"/> we're referencing, at runtime the instance can be found in the <see cref="StaticDatabase"/>
        /// </summary>
        public readonly string InstanceName;

        public StaticDataReference(Type type, string instanceName)
        {
            Type         = type;
            InstanceName = instanceName;
        }
    }
}