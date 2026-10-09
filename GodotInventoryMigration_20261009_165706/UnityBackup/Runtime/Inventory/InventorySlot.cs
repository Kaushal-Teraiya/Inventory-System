using InventorySystem.Domain;

namespace InventorySystem.Inventory
{
    public sealed class InventorySlot
    {
        public int Index { get; }

        public ItemStack Stack { get; private set; }

        public bool IsEmpty => Stack == null;

        public InventorySlot(int index)
        {
            Index = index;
        }

        public void SetStack(ItemStack stack)
        {
            Stack = stack;
        }

        public void Clear()
        {
            Stack = null;
        }
    }
}
