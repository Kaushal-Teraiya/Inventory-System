using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using NUnit.Framework;
using UnityEngine;

namespace InventorySystem.Tests
{
    public class InventoryTransactionIntegrationTests
    {
        private ItemDefinition CreateItem(
            string id,
            int maxStackSize = 10)
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();

            var serializedObject = new UnityEditor.SerializedObject(definition);
            serializedObject.FindProperty("itemId").stringValue = id;
            serializedObject.FindProperty("maxStackSize").intValue = maxStackSize;
            serializedObject.ApplyModifiedProperties();

            return definition;
        }

        [Test]
        public void TransferToTransactional_MovesItemSuccessfully()
        {
            var definition = CreateItem("Potion");
            var item = new ItemInstance(definition);

            var source = new _Inventory(5);
            var target = new _Inventory(5);

            source.AddItem(item, 5);

            var result = source.TransferToTransactional(
                target,
                item,
                3);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(source.GetItemQuantity(item), Is.EqualTo(2));
            Assert.That(target.GetItemQuantity(item), Is.EqualTo(3));

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void TransferToTransactional_FailsWhenSourceLacksQuantity()
        {
            var definition = CreateItem("Potion");
            var item = new ItemInstance(definition);

            var source = new _Inventory(5);
            var target = new _Inventory(5);

            source.AddItem(item, 2);

            var result = source.TransferToTransactional(
                target,
                item,
                5);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(source.GetItemQuantity(item), Is.EqualTo(2));
            Assert.That(target.GetItemQuantity(item), Is.EqualTo(0));

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void TransferToTransactional_RollsBackSourceWhenTargetFails()
        {
            var definition = CreateItem("Potion", 5);
            var otherDefinition = CreateItem("Sword", 1);

            var item = new ItemInstance(definition);
            var otherItem = new ItemInstance(otherDefinition);

            var source = new _Inventory(5);
            var target = new _Inventory(1);

            source.AddItem(item, 5);
            target.AddItem(otherItem, 1);

            var result = source.TransferToTransactional(
                target,
                item,
                5);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(source.GetItemQuantity(item), Is.EqualTo(5));
            Assert.That(target.GetItemQuantity(item), Is.EqualTo(0));
            Assert.That(target.GetItemQuantity(otherItem), Is.EqualTo(1));

            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(otherDefinition);
        }
    }
}


