using UnityEngine;

namespace InventorySystem.Items
{
    [CreateAssetMenu(fileName = "HealingItemEffect", menuName = "Inventory System/Effects/Healing")]
    public sealed class HealingItemEffect : ItemUseEffect
    {
        [SerializeField, Min(1)] private int healAmount = 25;

        public override bool TryApply(Health health, out string message)
        {
            if (health == null)
            {
                message = "No health target is available.";
                return false;
            }

            if (!health.IsAlive)
            {
                message = "Healing is unavailable while the player is down.";
                return false;
            }

            int restored = health.Heal(healAmount);

            if (restored <= 0)
            {
                message = "No health restored. Health may already be full.";
                return false;
            }

            message = $"+{restored} HP";
            return true;
        }
    }
}
