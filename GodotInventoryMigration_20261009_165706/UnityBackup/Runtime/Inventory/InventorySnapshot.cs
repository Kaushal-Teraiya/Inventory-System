using System;
using InventorySystem.Domain;

namespace InventorySystem.Inventory
{
    public sealed class InventorySnapshot
    {
        internal SnapshotSlot[] Slots { get; }

        internal InventorySnapshot(SnapshotSlot[] slots)
        {
            Slots = slots;
        }

        internal sealed class SnapshotSlot
        {
            public int Index { get; }
            public ItemInstance Item { get; }
            public int Quantity { get; }

            public SnapshotSlot(
                int index,
                ItemInstance item,
                int quantity)
            {
                Index = index;
                Item = item;
                Quantity = quantity;
            }
        }
    }
}
