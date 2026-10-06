using System.Reflection;
using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using NUnit.Framework;
using UnityEngine;

namespace InventorySystem.Tests
{
    public class InventoryRemovalTests
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
        public void RemoveItem_RemovesFromStack()
        {
            var inventory = new _Inventory(2);
            var potion = new ItemInstance(potionDefinition);

            inventory.AddItem(potion, 5);
            var result = inventory.RemoveItem(potion, 2);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(2, result.AffectedQuantity);
            Assert.AreEqual(3, inventory.Slots[0].Stack.Quantity);
        }

        [Test]
        public void RemoveItem_RemovesAcrossMultipleStacks()
        {
            var inventory = new _Inventory(2);
            var potion = new ItemInstance(potionDefinition);

            inventory.AddItem(potion, 8);
            var result = inventory.RemoveItem(potion, 6);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(6, result.AffectedQuantity);
            Assert.IsTrue(inventory.Slots[0].IsEmpty);
            Assert.AreEqual(2, inventory.Slots[1].Stack.Quantity);
        }

        [Test]
        public void RemoveItem_ClearsEmptyStack()
        {
            var inventory = new _Inventory(1);
            var potion = new ItemInstance(potionDefinition);

            inventory.AddItem(potion, 5);
            var result = inventory.RemoveItem(potion, 5);

            Assert.IsTrue(result.Succeeded);
            Assert.IsTrue(inventory.Slots[0].IsEmpty);
            Assert.IsNull(inventory.Slots[0].Stack);
        }

        [Test]
        public void RemoveItem_FailsWithoutChangingInventoryWhenInsufficient()
        {
            var inventory = new _Inventory(1);
            var potion = new ItemInstance(potionDefinition);

            inventory.AddItem(potion, 3);
            var result = inventory.RemoveItem(potion, 5);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed, result.Failure);
            Assert.AreEqual(3, inventory.Slots[0].Stack.Quantity);
        }

        [Test]
        public void RemoveItem_RejectsNullItem()
        {
            var inventory = new _Inventory(1);

            var result = inventory.RemoveItem(null, 1);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(InventoryOperationResult.InventoryOperationFailure.InvalidItem, result.Failure);
        }

        [Test]
        public void RemoveItem_RejectsInvalidQuantity()
        {
            var inventory = new _Inventory(1);
            var potion = new ItemInstance(potionDefinition);

            var result = inventory.RemoveItem(potion, 0);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(InventoryOperationResult.InventoryOperationFailure.InvalidQuantity, result.Failure);
        }

        [Test]
        public void RemoveItem_DoesNotRemoveDifferentItem()
        {
            var inventory = new _Inventory(2);
            var potion = new ItemInstance(potionDefinition);
            var sword = new ItemInstance(swordDefinition);

            inventory.AddItem(potion, 3);
            var result = inventory.RemoveItem(sword, 1);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed, result.Failure);
            Assert.AreEqual(3, inventory.Slots[0].Stack.Quantity);
        }

        private static ItemDefinition CreateDefinition(string id, int maxStackSize)
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();
            SetPrivateField(definition, "itemId", id);
            SetPrivateField(definition, "maxStackSize", maxStackSize);
            return definition;
        }

        private static void SetPrivateField<T>(ItemDefinition definition, string fieldName, T value)
        {
            typeof(ItemDefinition).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(definition, value);
        }
    }
}