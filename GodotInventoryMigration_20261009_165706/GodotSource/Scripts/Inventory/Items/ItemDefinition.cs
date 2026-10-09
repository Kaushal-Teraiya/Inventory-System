using Godot;
using InventorySystem.Domain;

namespace InventorySystem.Items
{
    [GlobalClass]
    public partial class ItemDefinition : Resource
    {
        [Export] public string ItemId { get; set; } = "";
        [Export] public string DisplayName { get; set; } = "";
        [Export] public Texture2D Icon { get; set; }
        [Export] public float Weight { get; set; } = 0f;
        [Export(PropertyHint.Range, "0,100,1")] public int EffectAmount { get; set; } = 25;
        [Export] public ItemUseEffect UseEffect { get; set; }

        private int _maxStackSize = 1;

        [Export]
        public int MaxStackSize
        {

            get => _maxStackSize;
            set => _maxStackSize = Mathf.Max(1, value);
        }

        public ItemId Id => new(ItemId);

        public bool IsValid =>
            Id.IsValid &&
            MaxStackSize >= 1 &&
            Weight >= 0f;
    }
}


