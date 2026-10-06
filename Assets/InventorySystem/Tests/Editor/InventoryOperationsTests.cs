using System.Reflection;
using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using NUnit.Framework;
using UnityEngine;

namespace InventorySystem.Tests
{
    public class InventoryOperationsTests
    {
        private ItemDefinition potionDefinition;
        private ItemDefinition swordDefinition;

        [SetUp]
        public void SetUp()
        {
            potionDefinition = CreateDefinition("potion", 5);
            swordDefinition = CreateDefinition("sword", 1);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(potionDefinition);
            Object.DestroyImmediate(swordDefinition);
        }

        [Test]
        public void AddItem_AddsItemToEmptySlot()
        {
            var inventory = new _Inventory(3);
            var potion = new ItemInstance(potionDefinition);

            var result = inventory.AddItem(potion, 3);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(3, result.AffectedQuantity);
            Assert.IsNotNull(inventory.Slots[0].Stack);
            Assert.AreSame(potion, inventory.Slots[0].Stack.Item);
            Assert.AreEqual(3, inventory.Slots[0].Stack.Quantity);
        }

        [Test]
        public void AddItem_AddsToExistingCompatibleStack()
        {
            var inventory = new _Inventory(3);
            var firstPotion = new ItemInstance(potionDefinition);
            var secondPotion = new ItemInstance(potionDefinition);

            inventory.AddItem(firstPotion, 3);

            var result = inventory.AddItem(secondPotion, 2);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(2, result.AffectedQuantity);
            Assert.AreEqual(5, inventory.Slots[0].Stack.Quantity);
            Assert.AreEqual(1, CountOccupiedSlots(inventory));
        }

        [Test]
        public void AddItem_CreatesMultipleStacksWhenNecessary()
        {
            var inventory = new _Inventory(3);
            var potion = new ItemInstance(potionDefinition);

            var result = inventory.AddItem(potion, 12);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(12, result.AffectedQuantity);

            Assert.AreEqual(5, inventory.Slots[0].Stack.Quantity);
            Assert.AreEqual(5, inventory.Slots[1].Stack.Quantity);
            Assert.AreEqual(2, inventory.Slots[2].Stack.Quantity);
        }

        [Test]
        public void AddItem_RejectsInvalidQuantity()
        {
            var inventory = new _Inventory(3);
            var potion = new ItemInstance(potionDefinition);

            var result = inventory.AddItem(potion, 0);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(
                InventoryOperationResult.InventoryOperationFailure.InvalidQuantity,
                result.Failure);
        }

        [Test]
        public void AddItem_RejectsNullItem()
        {
            var inventory = new _Inventory(3);

            var result = inventory.AddItem(null, 1);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(
                InventoryOperationResult.InventoryOperationFailure.InvalidItem,
                result.Failure);
        }

        [Test]
        public void AddItem_FailsWithoutChangingInventoryWhenFull()
        {
            var inventory = new _Inventory(1);
            var potion = new ItemInstance(potionDefinition);

            inventory.AddItem(potion, 5);

            var result = inventory.AddItem(potion, 1);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(
                InventoryOperationResult.InventoryOperationFailure.InventoryFull,
                result.Failure);

            Assert.AreEqual(5, inventory.Slots[0].Stack.Quantity);
        }

        [Test]
        public void AddItem_DoesNotStackDifferentItems()
        {
            var inventory = new _Inventory(2);
            var potion = new ItemInstance(potionDefinition);
            var sword = new ItemInstance(swordDefinition);

            inventory.AddItem(potion, 1);
            var result = inventory.AddItem(sword, 1);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(2, CountOccupiedSlots(inventory));
            Assert.AreSame(potion, inventory.Slots[0].Stack.Item);
            Assert.AreSame(sword, inventory.Slots[1].Stack.Item);
        }

        private static int CountOccupiedSlots(_Inventory inventory)
        {
            int count = 0;

            foreach (var slot in inventory.Slots)
            {
                if (!slot.IsEmpty)
                {
                    count++;
                }
            }

            return count;
        }

        private static ItemDefinition CreateDefinition(
            string id,
            int maxStackSize)
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();

            SetPrivateField(definition, "itemId", id);
            SetPrivateField(definition, "maxStackSize", maxStackSize);

            return definition;
        }

        private static void SetPrivateField<T>(
            ItemDefinition definition,
            string fieldName,
            T value)
        {
            var field = typeof(ItemDefinition).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            field.SetValue(definition, value);
        }
    }
}
