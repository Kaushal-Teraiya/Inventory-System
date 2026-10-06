using System;
using InventorySystem.Domain;
using NUnit.Framework;

namespace InventorySystem.Tests
{
    public class ItemInstanceTests
    {
        [Test]
        public void ItemInstance_RejectsNullDefinition()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ItemInstance(null));
        }
    }
}