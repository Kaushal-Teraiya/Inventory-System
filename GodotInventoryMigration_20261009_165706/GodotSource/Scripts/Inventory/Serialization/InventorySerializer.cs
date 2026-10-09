using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;

namespace InventorySystem.Serialization
{
    public static class InventorySerializer
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            IncludeFields = true,
            WriteIndented = true
        };

        public static string Serialize(_Inventory inventory)
        {
            ArgumentNullException.ThrowIfNull(inventory);

            var data = new InventorySaveData
            {
                Slots = inventory.Slots
                    .Where(slot => !slot.IsEmpty)
                    .Select(slot => new InventorySlotSaveData
                    {
                        SlotIndex = slot.Index,
                        ItemId = slot.Stack.Item.Definition.Id.Value,
                        Quantity = slot.Stack.Quantity
                    })
                    .ToArray()
            };

            return JsonSerializer.Serialize(data, JsonOptions);
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
                data = JsonSerializer.Deserialize<InventorySaveData>(
                    json, JsonOptions);
            }
            catch (JsonException)
            {
                return false;
            }

            if (data?.Slots == null)
                return false;

            var resolvedItems = new List<(int SlotIndex, ItemInstance Item, int Quantity)>();
            var occupiedIndices = new HashSet<int>();

            foreach (var savedSlot in data.Slots)
            {
                if (savedSlot == null ||
                    savedSlot.SlotIndex < 0 ||
                    savedSlot.SlotIndex >= inventory.Capacity ||
                    string.IsNullOrWhiteSpace(savedSlot.ItemId) ||
                    savedSlot.Quantity <= 0 ||
                    !occupiedIndices.Add(savedSlot.SlotIndex))
                    return false;

                ItemDefinition definition;

                try
                {
                    definition = itemResolver(savedSlot.ItemId);
                }
                catch
                {
                    return false;
                }

                if (definition == null || !definition.IsValid)
                    return false;

                if (savedSlot.Quantity > definition.MaxStackSize)
                    return false;

                resolvedItems.Add((
                    savedSlot.SlotIndex,
                    new ItemInstance(definition),
                    savedSlot.Quantity));
            }

            inventory.Clear();

            foreach (var entry in resolvedItems)
            {
                var result = inventory.AddItemToSlot(
                    entry.SlotIndex,
                    entry.Item,
                    entry.Quantity);

                if (!result.Succeeded)
                {
                    inventory.Clear();
                    return false;
                }
            }

            return true;
        }
    }
}
