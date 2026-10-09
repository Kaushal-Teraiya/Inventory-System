
using System;

namespace InventorySystem.Domain
{
    public readonly struct ItemId : IEquatable<ItemId>
    {
        public string Value { get; }

        public ItemId(string value)
        {
            Value = value;
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Value);

        public bool Equals(ItemId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is ItemId other && Equals(other);

        public override int GetHashCode() => Value == null ? 0 : StringComparer.Ordinal.GetHashCode(Value);

        public static bool operator ==(ItemId left, ItemId right) => left.Equals(right);

        public static bool operator !=(ItemId left, ItemId right) => !left.Equals(right);

        public override string ToString() => Value;
    }
}
