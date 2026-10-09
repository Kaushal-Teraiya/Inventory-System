using UnityEngine;

namespace InventorySystem.Items
{
    public abstract class ItemUseEffect : ScriptableObject
    {
        public abstract bool CanApply(GameObject user, GameObject target, out string message);
        public abstract void Apply(GameObject user, GameObject target, out string message);
    }
}
