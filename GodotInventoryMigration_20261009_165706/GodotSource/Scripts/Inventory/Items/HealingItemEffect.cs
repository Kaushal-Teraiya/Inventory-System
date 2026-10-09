using Godot;
using InventorySystem.Player;

namespace InventorySystem.Items
{
    [GlobalClass]
    public partial class HealingItemEffect : ItemUseEffect
    {
        [Export(PropertyHint.Range, "1,100,1")]
        public int HealAmount { get; set; } = 25;

        public override bool CanApply(PlayerHealth target, out string message)
        {
            if (target == null || !target.IsAlive)
            {
                message = "Healing is unavailable while the player is down.";
                return false;
            }

            if (target.CurrentHealth >= target.MaxHealth)
            {
                message = "Health is already full. Item not consumed.";
                return false;
            }

            if (HealAmount <= 0)
            {
                message = "This item has no healing effect.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public override void Apply(PlayerHealth target, out string message)
        {
            int restored = target.Heal(HealAmount);
            message = restored > 0
                ? $"+{restored} HP restored."
                : "No health restored.";
        }
    }
}
