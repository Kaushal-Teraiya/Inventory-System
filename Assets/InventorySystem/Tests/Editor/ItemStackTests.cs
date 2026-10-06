using System;
using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using NUnit.Framework;
using UnityEngine;

namespace InventorySystem.Tests
{
    public class ItemStackTests
    {
        private ItemDefinition definition;
        private ItemInstance item;

        [SetUp]
        public void SetUp()
        {
            definition = ScriptableObject.CreateInstance<ItemDefinition>();
            item = new ItemInstance(definition);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(definition);
        }

        [Test]
        public void ItemStack_CreatesWithCorrectQuantity()
        {
            var stack = new ItemStack(item, 5);

            Assert.AreSame(item, stack.Item);
            Assert.AreEqual(5, stack.Quantity);
        }

        [Test]
        public void AddQuantity_IncreasesStackQuantity()
        {
            var stack = new ItemStack(item, 5);

            stack.AddQuantity(3);

            Assert.AreEqual(8, stack.Quantity);
        }

        [Test]
        public void TryRemoveQuantity_DecreasesStackQuantity()
        {
            var stack = new ItemStack(item, 5);

            bool removed = stack.TryRemoveQuantity(2);

            Assert.IsTrue(removed);
            Assert.AreEqual(3, stack.Quantity);
        }

        [Test]
        public void TryRemoveQuantity_FailsWhenRemovingMoreThanAvailable()
        {
            var stack = new ItemStack(item, 5);

            bool removed = stack.TryRemoveQuantity(6);

            Assert.IsFalse(removed);
            Assert.AreEqual(5, stack.Quantity);
        }

        [Test]
        public void ItemStack_RejectsZeroQuantity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new ItemStack(item, 0));
        }

        [Test]
        public void ItemStack_RejectsNegativeQuantity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new ItemStack(item, -1));
        }

        [Test]
        public void AddQuantity_RejectsZero()
        {
            var stack = new ItemStack(item, 5);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => stack.AddQuantity(0));
        }

        [Test]
        public void AddQuantity_RejectsNegativeAmount()
        {
            var stack = new ItemStack(item, 5);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => stack.AddQuantity(-1));
        }

        [Test]
        public void TryRemoveQuantity_RejectsZero()
        {
            var stack = new ItemStack(item, 5);

            Assert.IsFalse(stack.TryRemoveQuantity(0));
            Assert.AreEqual(5, stack.Quantity);
        }

        [Test]
        public void TryRemoveQuantity_RejectsNegativeAmount()
        {
            var stack = new ItemStack(item, 5);

            Assert.IsFalse(stack.TryRemoveQuantity(-1));
            Assert.AreEqual(5, stack.Quantity);
        }

        [Test]
        public void ItemStack_RejectsNullItem()
        {
            Assert.Throws<ArgumentNullException>(() => new ItemStack(null, 5));
        }
    }
}
