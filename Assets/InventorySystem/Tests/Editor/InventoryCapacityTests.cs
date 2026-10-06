using System;
using InventorySystem.Inventory;
using NUnit.Framework;

namespace InventorySystem.Tests
{
    public class InventoryCapacityTests
    {
        [Test]
        public void Inventory_AllowsZeroCapacity()
        {
            var inventory = new _Inventory(0);

            Assert.AreEqual(0, inventory.Capacity);
            Assert.AreEqual(0, inventory.Slots.Count);
        }

        [Test]
        public void Inventory_RejectsNegativeCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new _Inventory(-1));
        }

        [Test]
        public void Inventory_SlotIndexesAreSequential()
        {
            var inventory = new _Inventory(5);

            for (int i = 0; i < inventory.Capacity; i++)
                Assert.AreEqual(i, inventory.Slots[i].Index);
        }
    }
}
