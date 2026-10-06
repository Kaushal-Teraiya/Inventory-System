using System;

namespace InventorySystem.Serialization
{
    [Serializable]
    public sealed class InventorySaveData
    {
        public InventorySlotSaveData[] Slots;
    }

    [Serializable]
    public sealed class InventorySlotSaveData
    {
        public int SlotIndex;
        public string ItemId;
        public int Quantity;
    }
}
