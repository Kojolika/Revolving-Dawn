namespace Models
{
    /// <summary>
    /// TModels are the definitions of a game object in memory where TSerializable are the serialized version of the
    /// model. This is so we have a clean separation between serializable data and game data.
    /// <remarks>The intended use of this is to have the Tmodel being the type that inherits this interface.</remarks>
    /// </summary>
    public interface IModel<out TModel, TSerializable>
    {
        TSerializable Serialize();
        TModel Deserialize(TSerializable serializable);
    }
}