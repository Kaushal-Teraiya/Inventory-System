using Godot;
using InventorySystem.Player;

namespace InventorySystem.Items
{
    [GlobalClass]
    public abstract partial class ItemUseEffect : Resource
    {
        public abstract bool CanApply(PlayerHealth target, out string message);
        public abstract void Apply(PlayerHealth target, out string message);
    }
}
