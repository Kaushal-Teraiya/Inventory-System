using System.Collections.Generic;
using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using InventorySystem.Serialization;
using NUnit.Framework;
using UnityEngine;

namespace InventorySystem.Tests
{
    public class InventorySerializationTests
    {
        private readonly List<ItemDefinition> definitions = new();

        private ItemDefinition CreateDefinition(
            string id,
            int maxStackSize = 10,
            float weight = 1f)
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();
            definitions.Add(definition);

            var serialized = new UnityEditor.SerializedObject(definition);
            serialized.FindProperty("itemId").stringValue = id;
            serialized.FindProperty("maxStackSize").intValue = maxStackSize;
            serialized.FindProperty("weight").floatValue = weight;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            return definition;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var definition in definitions)
                Object.DestroyImmediate(definition);

            definitions.Clear();
        }

        [Test]
        public void Serialize_EmptyInventory_ReturnsValidJson()
        {
            var inventory = new _Inventory(3);

            string json = InventorySerializer.Serialize(inventory);

            Assert.IsFalse(string.IsNullOrWhiteSpace(json));
            Assert.IsTrue(json.Contains("Slots"));
        }

        [Test]
        public void Serialize_SavesSlotIndexItemIdAndQuantity()
        {
            var inventory = new _Inventory(3);
            var definition = CreateDefinition("potion", 10);
            var item = new ItemInstance(definition);

            inventory.AddItemToSlot(1, item, 5);

            string json = InventorySerializer.Serialize(inventory);

            Assert.IsTrue(json.Contains("potion"));
            Assert.IsTrue(json.Contains("5"));
        }

        [Test]
        public void Deserialize_RestoresInventory()
        {
            var source = new _Inventory(3);
            var definition = CreateDefinition("potion", 10, 2f);
            var item = new ItemInstance(definition);

            source.AddItemToSlot(1, item, 5);

            string json = InventorySerializer.Serialize(source);

            var target = new _Inventory(3);

            bool result = InventorySerializer.Deserialize(
                json,
                target,
                id => id == "potion" ? definition : null);

            Assert.IsTrue(result);
            Assert.IsTrue(target.GetSlot(0).IsEmpty);
            Assert.IsFalse(target.GetSlot(1).IsEmpty);
            Assert.AreEqual(5, target.GetSlot(1).Stack.Quantity);
            Assert.AreEqual("potion", target.GetSlot(1).Stack.Item.Definition.Id.Value);
            Assert.AreEqual(10f, target.CurrentWeight);
        }

        [Test]
        public void Deserialize_UsesItemResolver()
        {
            var source = new _Inventory(1);
            var definition = CreateDefinition("sword", 1);
            var item = new ItemInstance(definition);

            source.AddItemToSlot(0, item, 1);

            string json = InventorySerializer.Serialize(source);

            var target = new _Inventory(1);
            bool resolverCalled = false;

            bool result = InventorySerializer.Deserialize(
                json,
                target,
                id =>
                {
                    resolverCalled = true;
                    return id == "sword" ? definition : null;
                });

            Assert.IsTrue(result);
            Assert.IsTrue(resolverCalled);
        }

        [Test]
        public void Deserialize_FailsWithNullJson()
        {
            var inventory = new _Inventory(2);

            bool result = InventorySerializer.Deserialize(
                null,
                inventory,
                _ => null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Deserialize_FailsWithNullInventory()
        {
            bool result = InventorySerializer.Deserialize(
                "{}",
                null,
                _ => null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Deserialize_FailsWithNullResolver()
        {
            var inventory = new _Inventory(2);

            bool result = InventorySerializer.Deserialize(
                "{}",
                inventory,
                null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Deserialize_FailsWhenItemCannotBeResolved()
        {
            var source = new _Inventory(1);
            var definition = CreateDefinition("potion");
            var item = new ItemInstance(definition);

            source.AddItemToSlot(0, item, 1);

            string json = InventorySerializer.Serialize(source);

            var target = new _Inventory(1);

            bool result = InventorySerializer.Deserialize(
                json,
                target,
                _ => null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Deserialize_FailsWhenSlotIndexIsInvalid()
        {
            string json =
                "{\"Slots\":[{\"SlotIndex\":99,\"ItemId\":\"potion\",\"Quantity\":1}]}";

            var inventory = new _Inventory(2);
            var definition = CreateDefinition("potion");

            bool result = InventorySerializer.Deserialize(
                json,
                inventory,
                _ => definition);

            Assert.IsFalse(result);
        }

        [Test]
        public void Deserialize_FailsWhenQuantityIsInvalid()
        {
            string json =
                "{\"Slots\":[{\"SlotIndex\":0,\"ItemId\":\"potion\",\"Quantity\":0}]}";

            var inventory = new _Inventory(2);
            var definition = CreateDefinition("potion");

            bool result = InventorySerializer.Deserialize(
                json,
                inventory,
                _ => definition);

            Assert.IsFalse(result);
        }

        [Test]
        public void Deserialize_ClearsExistingInventory()
        {
            var source = new _Inventory(2);
            var potion = CreateDefinition("potion");
            var sword = CreateDefinition("sword");

            source.AddItemToSlot(0, new ItemInstance(potion), 2);

            string json = InventorySerializer.Serialize(source);

            var target = new _Inventory(2);
            target.AddItemToSlot(1, new ItemInstance(sword), 1);

            bool result = InventorySerializer.Deserialize(
                json,
                target,
                id =>
                {
                    if (id == "potion") return potion;
                    if (id == "sword") return sword;
                    return null;
                });

            Assert.IsTrue(result);
            Assert.IsFalse(target.GetSlot(0).IsEmpty);
            Assert.IsTrue(target.GetSlot(1).IsEmpty);
        }
    }
}
