using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using NUnit.Framework;
using UnityEngine;

namespace InventorySystem.Tests
{
    public class InventorySnapshotTests
    {
        private ItemDefinition CreateItem(string id, int maxStackSize = 10)
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();

            var serializedObject = new UnityEditor.SerializedObject(definition);
            serializedObject.FindProperty("itemId").stringValue = id;
            serializedObject.FindProperty("maxStackSize").intValue = maxStackSize;
            serializedObject.ApplyModifiedProperties();

            return definition;
        }

        [Test]
        public void Snapshot_RestoresItemsAndQuantities()
        {
            var definition = CreateItem("Potion");
            var item = new ItemInstance(definition);
            var inventory = new _Inventory(5);

            inventory.AddItemToSlot(2, item, 5);

            var snapshot = inventory.CreateSnapshot();

            inventory.Clear();

            inventory.RestoreSnapshot(snapshot);

            Assert.That(inventory.GetItemQuantity(item), Is.EqualTo(5));
            Assert.That(inventory.GetSlot(2).Stack.Quantity, Is.EqualTo(5));

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void Snapshot_RestoresEmptyInventory()
        {
            var definition = CreateItem("Potion");
            var item = new ItemInstance(definition);
            var inventory = new _Inventory(5);

            inventory.AddItem(item, 3);

            var snapshot = inventory.CreateSnapshot();

            inventory.Clear();
            inventory.RestoreSnapshot(snapshot);

            Assert.That(inventory.GetItemQuantity(item), Is.EqualTo(3));

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void RestoreSnapshot_Null_Throws()
        {
            var inventory = new _Inventory(5);

            Assert.Throws<System.ArgumentNullException>(() =>
                inventory.RestoreSnapshot(null));
        }
    }
}

