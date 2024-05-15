namespace Models.Characters
{
    public interface IInventory
    {
        System.Collections.Generic.List<IItem> Inventory { get; }
    }

    public interface IItem
    {
        string Name { get; }
        int Quantity { get; }
        string Description { get; }
        UnityEngine.Sprite Sprite { get; }
    }
}