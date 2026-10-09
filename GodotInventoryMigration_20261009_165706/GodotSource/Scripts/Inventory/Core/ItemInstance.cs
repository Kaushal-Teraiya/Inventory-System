using System;
using InventorySystem.Items;

namespace InventorySystem.Domain
{
    public sealed class ItemInstance
    {
        public ItemDefinition Definition { get; }

        public ItemInstance(ItemDefinition definition)
        {
            Definition = definition??throw new ArgumentNullException(nameof(definition));
        }
    }
}