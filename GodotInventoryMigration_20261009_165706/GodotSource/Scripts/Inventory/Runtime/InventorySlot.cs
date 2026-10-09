namespace InventorySystem.Inventory
{
    public sealed class InventorySlot
    {
        public int Index { get; }
        public ItemStack Stack { get; private set; }
        public bool IsEmpty => Stack == null;
        public bool IsLocked { get; private set; }

        public InventorySlot(int index, bool initiallyLocked = false)
        {
            Index = index;
            IsLocked = initiallyLocked;
        }

        public bool TryLock()
        {
            if (IsLocked || !IsEmpty)
                return false;

            IsLocked = true;
            return true;
        }

        public bool Unlock()
        {
            if (!IsLocked)
                return false;

            IsLocked = false;
            return true;
        }

        public void SetStack(ItemStack stack)
        {
            if (IsLocked && stack != null)
                throw new System.InvalidOperationException(
                    $"Cannot place items into locked slot {Index}.");

            Stack = stack;
        }

        public void Clear()
        {
            Stack = null;
        }
    }
}
