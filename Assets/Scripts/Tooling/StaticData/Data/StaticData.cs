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
        public readonly string FullTypeName;
        public readonly string InstanceName;

        public StaticDataReference(string fullTypeName, string instanceName)
        {
            FullTypeName = fullTypeName;
            InstanceName = instanceName;
        }

        public bool IsReferenceValid() => !string.IsNullOrEmpty(FullTypeName) && !string.IsNullOrEmpty(InstanceName);
    }
}