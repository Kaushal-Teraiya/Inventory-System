using InventorySystem.Domain;
using InventorySystem.Inventory;

namespace InventorySystem.Rules
{
    public static class InventoryRules
    {
        public static bool AreItemsStackCompatible(ItemInstance first, ItemInstance second)
        {
            if (first == null || second == null)
            {
                return false;
            }

            return first.Definition.Id == second.Definition.Id;
        }

        public static int GetRemainingStackCapacity(ItemStack stack)
        {
            if (stack == null)
            {
                return 0;
            }

            int maxStackSize = stack.Item.Definition.MaxStackSize;

            int remaining = maxStackSize - stack.Quantity;

            return remaining > 0 ? remaining : 0;
        }
    }
}
