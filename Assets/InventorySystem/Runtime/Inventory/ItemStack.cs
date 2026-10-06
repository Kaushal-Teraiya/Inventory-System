using System;
using InventorySystem.Domain;

namespace InventorySystem.Inventory
{
    public sealed class ItemStack
    {
        public ItemInstance Item { get; }

        public int Quantity { get; private set; }

        public ItemStack(ItemInstance item, int quantity)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "Item stack quantity must be greater than zero.");

            Item = item;
            Quantity = quantity;
        }
        public void AddQuantity(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Amount to add must be greater than zero.");
            }

            Quantity += amount;
        }

        public bool TryRemoveQuantity(int amount)
        {
            if (amount <= 0 || amount > Quantity)
            {
                return false;
            }

            Quantity -= amount;
            return true;
        }
    }
}