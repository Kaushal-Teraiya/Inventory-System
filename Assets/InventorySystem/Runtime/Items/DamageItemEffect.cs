using UnityEngine;

namespace InventorySystem.Items
{
    [CreateAssetMenu(fileName = "DamageItemEffect", menuName = "Inventory System/Effects/Damage")]
    public sealed class DamageItemEffect : ItemUseEffect
    {
        [SerializeField, Min(1)] private int damageAmount = 25;

        public override bool TryApply(Health health, out string message)
        {
            if (health == null)
            {
                message = "No health target is available.";
                return false;
            }

            if (!health.IsAlive)
            {
                message = "The target is already down.";
                return false;
            }

            int dealt = health.TakeDamage(damageAmount);

            if (dealt <= 0)
            {
                message = "No damage was dealt.";
                return false;
            }

            message = $"Dealt {dealt} damage.";
            return true;
        }
    }
}
