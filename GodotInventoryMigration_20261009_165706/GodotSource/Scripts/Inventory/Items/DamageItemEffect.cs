using Godot;
using InventorySystem.Player;

namespace InventorySystem.Items
{
    [GlobalClass]
    public partial class DamageItemEffect : ItemUseEffect
    {
        [Export(PropertyHint.Range, "1,1000,1")]
        public int DamageAmount { get; set; } = 25;

        public override bool CanApply(PlayerHealth target, out string message)
        {
            if (target == null)
            {
                message = "No damage target was provided.";
                return false;
            }

            if (!target.IsAlive)
            {
                message = "The target is already dead.";
                return false;
            }

            if (DamageAmount <= 0)
            {
                message = "Damage must be greater than zero.";
                return false;
            }

            message = "";
            return true;
        }

        public override void Apply(PlayerHealth target, out string message)
        {
            if (!CanApply(target, out message))
                return;

            int damageDealt = target.TakeDamage(DamageAmount);
            message = $"Dealt {damageDealt} damage.";
        }
    }
}
