using Newtonsoft.Json;

namespace Tooling.StaticData
{
    public abstract class StaticData
    {
        // we're using the name as file names, no need to serialize again
        [JsonIgnore]
        public string Name;
    }
}