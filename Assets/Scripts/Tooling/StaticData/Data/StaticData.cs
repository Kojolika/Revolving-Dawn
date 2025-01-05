using System;
using Newtonsoft.Json;
using Tooling.StaticData.Validation;

namespace Tooling.StaticData
{
    public abstract class StaticData
    {
        // we're using the name as file names, no need to serialize again
        [JsonIgnore, Required(allowDefaultValues: false), Unique]
        public string Name;

        [JsonIgnore] public StaticDataReference SerializedReference { get; set; }
    }

    public class StaticDataReference
    {
        public Type Type;
        public readonly string InstanceName;

        public StaticDataReference(Type type, string instanceName)
        {
            Type = type;
            InstanceName = instanceName;
        }

        public bool IsReferenceValid() => Type != null && !string.IsNullOrEmpty(InstanceName);
    }
}