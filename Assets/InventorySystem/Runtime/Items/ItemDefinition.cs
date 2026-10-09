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
        [SerializeField, Min(1)] private int maxStackSize = 1;
        [SerializeField, Min(0f)] private float weight;
        [SerializeField] private ItemUseEffect useEffect;

        public ItemId Id => new(itemId);
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public int MaxStackSize => maxStackSize;
        public float Weight => weight;
        public ItemUseEffect UseEffect => useEffect;

        public bool IsValid =>
            Id.IsValid &&
            maxStackSize >= 1 &&
            weight >= 0f;

        private void OnValidate()
        {
            maxStackSize = Mathf.Max(1, maxStackSize);
            weight = Mathf.Max(0f, weight);
        }
    }
}
