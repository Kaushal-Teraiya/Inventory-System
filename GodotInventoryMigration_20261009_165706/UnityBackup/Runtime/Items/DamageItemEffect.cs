using UnityEngine;

namespace InventorySystem.Items
{
    [CreateAssetMenu(menuName = "Inventory System/Effects/Damage")]
    public sealed class DamageItemEffect : ItemUseEffect
    {
        [SerializeField, Min(1)] private int damageAmount = 25;

        public override bool CanApply(GameObject user, GameObject target, out string message)
        {
            Health health = target != null ? target.GetComponent<Health>() : null;
            bool valid = health != null && health.IsAlive && damageAmount > 0;
            message = valid ? "" : "Invalid damage target.";
            return valid;
        }

        public override void Apply(GameObject user, GameObject target, out string message)
        {
            if (!CanApply(user, target, out message)) return;
            int dealt = target.GetComponent<Health>().TakeDamage(damageAmount);
            message = $"Dealt {dealt} damage.";
        }
    }
}
