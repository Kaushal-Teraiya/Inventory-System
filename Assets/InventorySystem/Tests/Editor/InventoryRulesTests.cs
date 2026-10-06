using System.Reflection;
using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using InventorySystem.Rules;
using NUnit.Framework;
using UnityEngine;

namespace InventorySystem.Tests
{
    public class InventoryRulesTests
    {
        private ItemDefinition firstDefinition;
        private ItemDefinition secondDefinition;

        [SetUp]
        public void SetUp()
        {
            firstDefinition = CreateDefinition("potion", 10);
            secondDefinition = CreateDefinition("sword", 1);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(firstDefinition);
            Object.DestroyImmediate(secondDefinition);
        }

        [Test]
        public void AreItemsStackCompatible_ReturnsTrueForSameItemId()
        {
            var first = new ItemInstance(firstDefinition);

            var duplicateDefinition = CreateDefinition("potion", 10);
            var second = new ItemInstance(duplicateDefinition);

            Assert.IsTrue(
                InventoryRules.AreItemsStackCompatible(first, second));

            Object.DestroyImmediate(duplicateDefinition);
        }

        [Test]
        public void AreItemsStackCompatible_ReturnsFalseForDifferentItemId()
        {
            var first = new ItemInstance(firstDefinition);
            var second = new ItemInstance(secondDefinition);

            Assert.IsFalse(
                InventoryRules.AreItemsStackCompatible(first, second));
        }

        [Test]
        public void AreItemsStackCompatible_ReturnsFalseForNullItem()
        {
            var item = new ItemInstance(firstDefinition);

            Assert.IsFalse(
                InventoryRules.AreItemsStackCompatible(item, null));

            Assert.IsFalse(
                InventoryRules.AreItemsStackCompatible(null, item));
        }

        [Test]
        public void GetRemainingStackCapacity_ReturnsRemainingCapacity()
        {
            var item = new ItemInstance(firstDefinition);
            var stack = new ItemStack(item, 4);

            int remaining = InventoryRules.GetRemainingStackCapacity(stack);

            Assert.AreEqual(6, remaining);
        }

        [Test]
        public void GetRemainingStackCapacity_ReturnsZeroWhenStackIsFull()
        {
            var item = new ItemInstance(firstDefinition);
            var stack = new ItemStack(item, 10);

            int remaining = InventoryRules.GetRemainingStackCapacity(stack);

            Assert.AreEqual(0, remaining);
        }

        [Test]
        public void GetRemainingStackCapacity_ReturnsZeroForNullStack()
        {
            Assert.AreEqual(
                0,
                InventoryRules.GetRemainingStackCapacity(null));
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
