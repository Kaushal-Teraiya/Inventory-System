using UnityEngine;

namespace InventorySystem.Items
{
    public abstract class ItemUseEffect : ScriptableObject
    {
        public abstract bool TryApply(Health health, out string message);
    }
}
