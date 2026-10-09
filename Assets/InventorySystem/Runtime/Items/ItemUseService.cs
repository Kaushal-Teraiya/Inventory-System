using InventorySystem.Inventory;
using UnityEngine;

namespace InventorySystem.Items
{
    public sealed class ItemUseService
    {
        public bool TryUse(
            _Inventory inventory,
            int slotIndex,
            Health health,
            out string message)
        {
            message = string.Empty;

            if (inventory == null)
            {
                message = "Inventory is unavailable.";
                return false;
            }

            var slot = inventory.GetSlot(slotIndex);

            if (slot == null || slot.IsEmpty)
            {
                message = "Select an occupied slot.";
                return false;
            }

            var definition = slot.Stack.Item.Definition;
            var effect = definition.UseEffect;

            if (effect == null)
            {
                message = "This item has no use effect. Item not consumed.";
                return false;
            }

            // Apply the effect only after validating the item and target.
            if (!effect.TryApply(health, out message))
                return false;

            // Consume one item only after successful use.
            var removal = inventory.RemoveFromSlot(slotIndex, 1);

            if (!removal.Succeeded)
            {
                Debug.LogError(
                    "The effect succeeded, but inventory consumption failed. " +
                    "Check the inventory transaction implementation.");
                message = "Effect applied, but item consumption failed.";
                return false;
            }

            return true;
        }
    }
}
