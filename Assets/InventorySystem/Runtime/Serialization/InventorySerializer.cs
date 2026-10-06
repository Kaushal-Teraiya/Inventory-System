using System;
using System.Collections.Generic;
using InventorySystem.Inventory;
using InventorySystem.Domain;
using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem.Serialization
{
    public static class InventorySerializer
    {
        public static string Serialize(_Inventory inventory)
        {
            if (inventory == null)
                throw new ArgumentNullException(nameof(inventory));

            var slots = new List<InventorySlotSaveData>();

            foreach (var slot in inventory.Slots)
            {
                if (slot.IsEmpty)
                    continue;

                slots.Add(new InventorySlotSaveData
                {
                    SlotIndex = slot.Index,
                    ItemId = slot.Stack.Item.Definition.Id.Value,
                    Quantity = slot.Stack.Quantity
                });
            }

            return JsonUtility.ToJson(new InventorySaveData
            {
                Slots = slots.ToArray()
            });
        }

        public static bool Deserialize(
            string json,
            _Inventory inventory,
            Func<string, ItemDefinition> itemResolver)
        {
            if (string.IsNullOrWhiteSpace(json) ||
                inventory == null ||
                itemResolver == null)
                return false;

            InventorySaveData data;

            try
            {
                data = JsonUtility.FromJson<InventorySaveData>(json);
            }
            catch
            {
                return false;
            }

            if (data == null || data.Slots == null)
                return false;

            inventory.Clear();

            foreach (var savedSlot in data.Slots)
            {
                if (savedSlot == null ||
                    savedSlot.SlotIndex < 0 ||
                    savedSlot.SlotIndex >= inventory.Capacity ||
                    string.IsNullOrWhiteSpace(savedSlot.ItemId) ||
                    savedSlot.Quantity <= 0)
                    return false;

                var definition = itemResolver(savedSlot.ItemId);

                if (definition == null)
                    return false;

                var item = new ItemInstance(definition);

                var result = inventory.AddItemToSlot(
                    savedSlot.SlotIndex,
                    item,
                    savedSlot.Quantity);

                if (!result.Succeeded)
                    return false;
            }

            return true;
        }
    }
}
