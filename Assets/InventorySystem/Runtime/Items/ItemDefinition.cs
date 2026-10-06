using InventorySystem.Domain;
using UnityEngine;

namespace InventorySystem.Items
{
    [CreateAssetMenu(
        fileName = "ItemDefinition",
        menuName = "Inventory System/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField] private int maxStackSize = 1;
        [SerializeField] private float weight;

        public ItemId Id => new(itemId);
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public int MaxStackSize => maxStackSize;
        public float Weight => weight;

        private void OnValidate()
        {
            if (maxStackSize < 1) maxStackSize = 1;
        }
    }
}