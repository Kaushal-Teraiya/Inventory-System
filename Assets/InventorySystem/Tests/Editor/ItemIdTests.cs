using System.Collections.Generic;
using InventorySystem.Domain;
using NUnit.Framework;

namespace InventorySystem.Tests
{
    public class ItemIdTests
    {
        [Test]
        public void ItemId_EqualValues_AreEqual()
        {
            var first = new ItemId("potion");
            var second = new ItemId("potion");

            Assert.AreEqual(first, second);
        }

        [Test]
        public void ItemId_DifferentValues_AreNotEqual()
        {
            var first = new ItemId("potion");
            var second = new ItemId("sword");

            Assert.AreNotEqual(first, second);
        }

        [Test]
        public void ItemId_EqualValues_HaveSameHashCode()
        {
            var first = new ItemId("potion");
            var second = new ItemId("potion");

            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [Test]
        public void ItemId_CanBeUsedAsDictionaryKey()
        {
            var dictionary = new Dictionary<ItemId, string>();
            dictionary[new ItemId("potion")] = "Health Potion";

            Assert.IsTrue(dictionary.ContainsKey(new ItemId("potion")));
            Assert.AreEqual("Health Potion", dictionary[new ItemId("potion")]);
        }
    }
}
