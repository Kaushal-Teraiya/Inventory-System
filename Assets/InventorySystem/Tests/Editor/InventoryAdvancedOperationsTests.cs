using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using NUnit.Framework;
using UnityEngine;

namespace InventorySystem.Tests
{
    public class InventoryAdvancedOperationsTests
    {
        private ItemDefinition CreateItem(string id, int maxStackSize = 10, float weight = 1f)
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();

            var serialized = new UnityEditor.SerializedObject(definition);
            serialized.FindProperty("itemId").stringValue = id;
            serialized.FindProperty("maxStackSize").intValue = maxStackSize;
            serialized.FindProperty("weight").floatValue = weight;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            return definition;
        }

        private ItemInstance CreateInstance(string id, int maxStackSize = 10, float weight = 1f)
        {
            return new ItemInstance(CreateItem(id, maxStackSize, weight));
        }

        [Test]
        public void MoveItem_MovesStackToEmptySlot()
        {
            var inventory = new _Inventory(2);
            var item = CreateInstance("sword");

            inventory.AddItemToSlot(0, item, 5);

            var result = inventory.MoveItem(0, 1);

            Assert.IsTrue(result.Succeeded);
            Assert.IsTrue(inventory.GetSlot(0).IsEmpty);
            Assert.IsFalse(inventory.GetSlot(1).IsEmpty);
            Assert.AreEqual(5, inventory.GetSlot(1).Stack.Quantity);
        }

        [Test]
        public void MoveItem_MergesCompatibleStacks()
        {
            var inventory = new _Inventory(2);
            var item = CreateInstance("potion", 10);

            inventory.AddItemToSlot(0, item, 4);
            inventory.AddItemToSlot(1, item, 3);

            var result = inventory.MoveItem(0, 1);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(7, inventory.GetSlot(1).Stack.Quantity);
            Assert.IsTrue(inventory.GetSlot(0).IsEmpty);
        }

        [Test]
        public void MoveItem_PartiallyMergesWhenTargetHasLimitedCapacity()
        {
            var inventory = new _Inventory(2);
            var item = CreateInstance("potion", 10);

            inventory.AddItemToSlot(0, item, 6);
            inventory.AddItemToSlot(1, item, 8);

            var result = inventory.MoveItem(0, 1);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(10, inventory.GetSlot(1).Stack.Quantity);
            Assert.AreEqual(4, inventory.GetSlot(0).Stack.Quantity);
        }

        [Test]
        public void MoveItem_SwapsIncompatibleStacks()
        {
            var inventory = new _Inventory(2);
            var sword = CreateInstance("sword");
            var potion = CreateInstance("potion");

            inventory.AddItemToSlot(0, sword, 1);
            inventory.AddItemToSlot(1, potion, 3);

            var result = inventory.MoveItem(0, 1);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual("potion", inventory.GetSlot(0).Stack.Item.Definition.Id.Value);
            Assert.AreEqual("sword", inventory.GetSlot(1).Stack.Item.Definition.Id.Value);
        }

        [Test]
        public void MoveItem_FailsWhenSourceIsEmpty()
        {
            var inventory = new _Inventory(2);

            var result = inventory.MoveItem(0, 1);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(
                InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed,
                result.Failure);
        }

        [Test]
        public void MoveItem_FailsForInvalidSlot()
        {
            var inventory = new _Inventory(2);

            var result = inventory.MoveItem(-1, 1);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(
                InventoryOperationResult.InventoryOperationFailure.InvalidSlot,
                result.Failure);
        }

        [Test]
        public void MoveItem_FailsWhenSourceAndTargetAreSame()
        {
            var inventory = new _Inventory(2);
            var item = CreateInstance("sword");

            inventory.AddItemToSlot(0, item, 1);

            var result = inventory.MoveItem(0, 0);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(
                InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed,
                result.Failure);
        }

        [Test]
        public void AddItemToSlot_AddsToEmptySlot()
        {
            var inventory = new _Inventory(2);
            var item = CreateInstance("potion", 10);

            var result = inventory.AddItemToSlot(1, item, 5);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(5, inventory.GetSlot(1).Stack.Quantity);
            Assert.AreEqual(5f, inventory.CurrentWeight);
        }

        [Test]
        public void AddItemToSlot_AddsToCompatibleStack()
        {
            var inventory = new _Inventory(2);
            var item = CreateInstance("potion", 10);

            inventory.AddItemToSlot(0, item, 4);

            var result = inventory.AddItemToSlot(0, item, 3);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(7, inventory.GetSlot(0).Stack.Quantity);
        }

        [Test]
        public void AddItemToSlot_FailsWhenStackCapacityIsExceeded()
        {
            var inventory = new _Inventory(1);
            var item = CreateInstance("potion", 5);

            inventory.AddItemToSlot(0, item, 4);

            var result = inventory.AddItemToSlot(0, item, 2);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(4, inventory.GetSlot(0).Stack.Quantity);
        }

        [Test]
        public void AddItemToSlot_FailsForIncompatibleItem()
        {
            var inventory = new _Inventory(1);
            var sword = CreateInstance("sword");
            var potion = CreateInstance("potion");

            inventory.AddItemToSlot(0, sword, 1);

            var result = inventory.AddItemToSlot(0, potion, 1);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("sword", inventory.GetSlot(0).Stack.Item.Definition.Id.Value);
        }

        [Test]
        public void AddItemToSlot_FailsForInvalidSlot()
        {
            var inventory = new _Inventory(2);
            var item = CreateInstance("potion");

            var result = inventory.AddItemToSlot(5, item, 1);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(
                InventoryOperationResult.InventoryOperationFailure.InvalidSlot,
                result.Failure);
        }

        [Test]
        public void AddItemToSlot_FailsForNullItem()
        {
            var inventory = new _Inventory(2);

            var result = inventory.AddItemToSlot(0, null, 1);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(
                InventoryOperationResult.InventoryOperationFailure.InvalidItem,
                result.Failure);
        }

        [Test]
        public void AddItemToSlot_FailsForInvalidQuantity()
        {
            var inventory = new _Inventory(2);
            var item = CreateInstance("potion");

            var result = inventory.AddItemToSlot(0, item, 0);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(
                InventoryOperationResult.InventoryOperationFailure.InvalidQuantity,
                result.Failure);
        }

        [Test]
        public void AddItemToSlot_RespectsWeightLimit()
        {
            var inventory = new _Inventory(2, 5f);
            var item = CreateInstance("rock", 10, 2f);

            var result = inventory.AddItemToSlot(0, item, 3);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(0f, inventory.CurrentWeight);
            Assert.IsTrue(inventory.GetSlot(0).IsEmpty);
        }

        [Test]
        public void SlotChanged_EventCanBeSubscribed()
        {
            var inventory = new _Inventory(1);
            var item = CreateInstance("potion");

            InventorySlot changedSlot = null;
            inventory.SlotChanged += slot => changedSlot = slot;

            inventory.AddItemToSlot(0, item, 1);

            Assert.IsNotNull(changedSlot);
            Assert.AreEqual(0, changedSlot.Index);
        }

        [Test]
        public void InventoryChanged_EventCanBeSubscribed()
        {
            var inventory = new _Inventory(1);
            var item = CreateInstance("potion");

            bool changed = false;
            inventory.InventoryChanged += () => changed = true;

            inventory.AddItemToSlot(0, item, 1);

            Assert.IsTrue(changed);
        }
    }
}