namespace Models
{
    public interface IRuntimeModel<D>
    {
        D Definition { get; }
    }
}