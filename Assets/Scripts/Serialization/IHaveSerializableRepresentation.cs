namespace Serialization
{
    public interface IHaveSerializableRepresentation<T>
    {
        T Representation { get; }
    }
}