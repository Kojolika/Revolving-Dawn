using Newtonsoft.Json;
using Tooling.StaticData.Validation;

namespace Tooling.StaticData
{
    public abstract class StaticData
    {
        // we're using the name as file names, no need to serialize again
        [JsonIgnore, Required(allowDefaultValues: false)]
        public string Name;
    }
}