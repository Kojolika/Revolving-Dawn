using Utils.Attributes;

namespace Data
{
    public interface IHaveAddressableKey
    {
        [ResourcePath]
        static string ResourcePath { get; }
    }
}