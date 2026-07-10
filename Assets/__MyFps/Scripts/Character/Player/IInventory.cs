namespace MyFps
{
    public interface IInventory
    {
        void ModifyItem(ItemType type, int amount);
        bool HasItem(ItemType type, int amount = 1);
    }
}