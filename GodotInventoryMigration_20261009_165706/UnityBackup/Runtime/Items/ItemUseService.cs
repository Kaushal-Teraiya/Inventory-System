using InventorySystem.Inventory;
using UnityEngine;

namespace InventorySystem.Items
{
    public sealed class ItemUseService
    {
        public bool TryUse(_Inventory inventory, int slotIndex,
            GameObject user, GameObject target, out string message)
        {
            var slot = inventory?.GetSlot(slotIndex);
            if (slot == null || slot.IsEmpty)
            {
                message = "Empty or invalid inventory slot.";
                return false;
            }

            ItemUseEffect effect = slot.Stack.Item.Definition.UseEffect;
            if (effect == null)
            {
                message = "This item has no assigned effect.";
                return false;
            }

            if (!effect.CanApply(user, target, out message))
                return false;

            // Consume only after validation; execute the effect after removal succeeds.
            var result = inventory.RemoveFromSlot(slotIndex, 1);
            if (!result.Succeeded)
            {
                message = "Failed to consume the item.";
                return false;
            }

            effect.Apply(user, target, out message);
            return true;
        }
    }
}
