using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using NUnit.Framework;
using UnityEngine;

namespace InventorySystem.Tests
{
    public class InventoryEventTests
    {
        private ItemInstance CreateItem(string id, int maxStackSize = 10)
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();

            var serialized = new UnityEditor.SerializedObject(definition);
            serialized.FindProperty("itemId").stringValue = id;
            serialized.FindProperty("maxStackSize").intValue = maxStackSize;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            return new ItemInstance(definition);
        }

        [Test]
        public void AddItem_FiresInventoryChanged()
        {
            var inventory = new _Inventory(2);
            var item = CreateItem("potion");
            int count = 0;

            inventory.InventoryChanged += () => count++;

            inventory.AddItem(item, 1);

            Assert.AreEqual(1, count);
        }

        [Test]
        public void RemoveItem_FiresInventoryChanged()
        {
            var inventory = new _Inventory(2);
            var item = CreateItem("potion");

            inventory.AddItem(item, 2);

            int count = 0;
            inventory.InventoryChanged += () => count++;

            inventory.RemoveItem(item, 1);

            Assert.AreEqual(1, count);
        }

        [Test]
        public void MoveItem_FiresSlotChangedForBothSlots()
        {
            var inventory = new _Inventory(2);
            var item = CreateItem("potion");

            inventory.AddItemToSlot(0, item, 2);

            int count = 0;
            inventory.SlotChanged += _ => count++;

            inventory.MoveItem(0, 1);

            Assert.AreEqual(2, count);
        }

        [Test]
        public void SplitStack_FiresSlotChangedForBothSlots()
        {
            var inventory = new _Inventory(2);
            var item = CreateItem("potion");

            inventory.AddItemToSlot(0, item, 5);

            int count = 0;
            inventory.SlotChanged += _ => count++;

            inventory.SplitStack(0, 1, 2);

            Assert.AreEqual(2, count);
        }

        [Test]
        public void RemoveFromSlot_FiresSlotChanged()
        {
            var inventory = new _Inventory(1);
            var item = CreateItem("potion");

            inventory.AddItemToSlot(0, item, 5);

            int count = 0;
            inventory.SlotChanged += _ => count++;

            inventory.RemoveFromSlot(0, 2);

            Assert.AreEqual(1, count);
        }

        [Test]
        public void Clear_FiresInventoryChanged()
        {
            var inventory = new _Inventory(2);
            var item = CreateItem("potion");

            inventory.AddItem(item, 2);

            int count = 0;
            inventory.InventoryChanged += () => count++;

            inventory.Clear();

            Assert.AreEqual(1, count);
        }

        [Test]
        public void Clear_FiresSlotChangedForEachOccupiedSlot()
        {
            var inventory = new _Inventory(2);
            var item = CreateItem("potion");

            inventory.AddItemToSlot(0, item, 1);
            inventory.AddItemToSlot(1, item, 1);

            int count = 0;
            inventory.SlotChanged += _ => count++;

            inventory.Clear();

            Assert.AreEqual(2, count);
        }
    }
}