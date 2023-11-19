using Utils;

namespace Data
{
    // TODO: Figure out if this is worth inherting from for all defintions
    // Rename IDefinition?
    public interface IModel
    {
        ReadOnly<string> ID { get; }
    }
}
