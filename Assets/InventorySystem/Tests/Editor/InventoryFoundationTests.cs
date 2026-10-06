using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using NUnit.Framework;
using UnityEngine;

namespace InventorySystem.Tests
{
    public class InventoryFoundationTests
    {
        [Test]
        public void Inventory_CreatesExpectedNumberOfSlots()
        {
            var inventory = new _Inventory(10);

            Assert.AreEqual(10, inventory.Capacity);
            Assert.AreEqual(10, inventory.Slots.Count);
        }

        [Test]
        public void InventorySlots_StartEmpty()
        {
            var inventory = new _Inventory(5);

            foreach (var slot in inventory.Slots)
            {
                Assert.IsTrue(slot.IsEmpty);
                Assert.IsNull(slot.Stack);
            }
        }

        [Test]
        public void InventorySlot_CanHoldItemStack()
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();
            var item = new ItemInstance(definition);
            var stack = new ItemStack(item, 5);

            var slot = new InventorySlot(0);

            slot.SetStack(stack);

            Assert.IsFalse(slot.IsEmpty);
            Assert.AreSame(stack, slot.Stack);

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void InventoryOperationResult_ReportsSuccess()
        {
            var result = InventoryOperationResult.Success(5);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(
                InventoryOperationResult.InventoryOperationFailure.None,
                result.Failure);

            Assert.AreEqual(5, result.AffectedQuantity);
        }

        [Test]
        public void InventoryOperationResult_ReportsFailure()
        {
            var result = InventoryOperationResult.Failed(
                InventoryOperationResult.InventoryOperationFailure.InventoryFull);

            Assert.IsFalse(result.Succeeded);

            Assert.AreEqual(
                InventoryOperationResult.InventoryOperationFailure.InventoryFull,
                result.Failure);

            Assert.AreEqual(0, result.AffectedQuantity);
        }
    }
}
