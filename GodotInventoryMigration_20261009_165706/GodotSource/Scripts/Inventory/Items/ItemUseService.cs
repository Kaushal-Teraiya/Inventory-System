using InventorySystem.Inventory;
using InventorySystem.Player;

namespace InventorySystem.Items
{
    public sealed class ItemUseService
    {
        public bool TryUse(
            _Inventory inventory,
            int slotIndex,
            PlayerHealth player,
            out string message)
        {
            if (inventory == null || player == null)
            {
                message = "Inventory or player health is unavailable.";
                return false;
            }

            var slot = inventory.GetSlot(slotIndex);
            if (slot == null || slot.IsEmpty || slot.IsLocked)
            {
                message = "Select an unlocked item first.";
                return false;
            }

            var definition = slot.Stack.Item.Definition;
            ItemUseEffect effect = definition.UseEffect;

            // Backward compatibility for existing items using HealAmount.
            if (effect == null && definition.EffectAmount > 0)
                effect = new HealingItemEffect { HealAmount = definition.EffectAmount };

            if (effect == null)
            {
                message = $"{definition.DisplayName} has no use effect configured.";
                return false;
            }

            if (!effect.CanApply(player, out message))
                return false;

            var removal = inventory.RemoveFromSlot(slotIndex, 1);
            if (!removal.Succeeded)
            {
                message = "Could not consume the selected item.";
                return false;
            }

            effect.Apply(player, out message);
            return true;
        }
    }
}
