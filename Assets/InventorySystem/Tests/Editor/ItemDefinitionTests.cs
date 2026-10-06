using System.Reflection;
using InventorySystem.Items;
using NUnit.Framework;
using UnityEngine;

namespace InventorySystem.Tests
{
    public class ItemDefinitionTests
    {
        [Test]
        public void ItemDefinition_DefaultMaxStackSize_IsOne()
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();

            Assert.AreEqual(1, definition.MaxStackSize);

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void ItemDefinition_MaxStackSize_CanBeConfigured()
        {
            var definition = CreateDefinition(10);

            Assert.AreEqual(10, definition.MaxStackSize);

            Object.DestroyImmediate(definition);
        }

        private static ItemDefinition CreateDefinition(int maxStackSize)
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();
            var field = typeof(ItemDefinition).GetField(
                "maxStackSize",
                BindingFlags.Instance | BindingFlags.NonPublic);

            field.SetValue(definition, maxStackSize);
            return definition;
        }

        [Test]
        public void ItemDefinition_MaxStackSize_ZeroIsClampedToOne()
        {
            var definition = CreateDefinition(0);

            InvokeOnValidate(definition);

            Assert.AreEqual(1, definition.MaxStackSize);
            Object.DestroyImmediate(definition);
        }

        [Test]
        public void ItemDefinition_MaxStackSize_NegativeIsClampedToOne()
        {
            var definition = CreateDefinition(-5);

            InvokeOnValidate(definition);

            Assert.AreEqual(1, definition.MaxStackSize);
            Object.DestroyImmediate(definition);
        }

        private static void InvokeOnValidate(ItemDefinition definition)
        {
            typeof(ItemDefinition).GetMethod(
                "OnValidate",
                BindingFlags.Instance | BindingFlags.NonPublic
            ).Invoke(definition, null);
        }

        [Test]
        public void ItemDefinition_Id_ReportsInvalidWhenEmpty()
        {
            var definition = CreateDefinition(5);
            Assert.IsFalse(definition.Id.IsValid);
            Object.DestroyImmediate(definition);
        }

        private static void SetItemId(ItemDefinition definition, string value)
        {
            typeof(ItemDefinition).GetField("itemId", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(definition, value);
        }
    }
}
