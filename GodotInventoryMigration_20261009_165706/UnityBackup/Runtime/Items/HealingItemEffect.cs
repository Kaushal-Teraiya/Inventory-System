using UnityEngine;

namespace InventorySystem.Items
{
    [CreateAssetMenu(menuName = "Inventory System/Effects/Healing")]
    public sealed class HealingItemEffect : ItemUseEffect
    {
        [SerializeField, Min(1)] private int healAmount = 25;

        public override bool CanApply(GameObject user, GameObject target, out string message)
        {
            Health health = target != null ? target.GetComponent<Health>() : null;
            bool valid = health != null && health.IsAlive &&
                         health.CurrentHealth < health.MaxHealth && healAmount > 0;
            message = valid ? "" : "Target cannot be healed.";
            return valid;
        }

        public override void Apply(GameObject user, GameObject target, out string message)
        {
            if (!CanApply(user, target, out message)) return;
            int restored = target.GetComponent<Health>().Heal(healAmount);
            message = $"Restored {restored} health.";
        }
    }
}
