using System;
using Godot;
using InventorySystem.Inventory;

namespace InventorySystem.UI
{
    public partial class InventorySlotUI : PanelContainer
    {
        public event Action<int> SlotSelected;
        public event Action<int, int> SplitRequested;
        public event Action<int, int> SlotMoveRequested;

        private TextureRect _icon;
        private TextureRect _lockIcon;
        private Label _quantity;
        private InventorySlot _boundSlot;
        private bool _selected;
        private StyleBoxFlat _normal, _hover, _pressed, _selectedStyle;

        public int SlotIndex => _boundSlot?.Index ?? -1;

        public override void _Ready()
        {
            _icon = GetNode<TextureRect>("SlotContent/ItemIcon");
            _quantity = GetNode<Label>("SlotContent/QuantityLabel");

            _lockIcon = new TextureRect
            {
                Name = "LockIcon",
                Texture = GD.Load<Texture2D>("res://Assets/Icons/padlock-512.png"),
                CustomMinimumSize = new Vector2(30, 30),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                MouseFilter = MouseFilterEnum.Ignore,
                Visible = false
            };
            _lockIcon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
            GetNode<Control>("SlotContent").AddChild(_lockIcon);
            _icon.MouseFilter = MouseFilterEnum.Ignore;
            _quantity.MouseFilter = MouseFilterEnum.Ignore;
            MouseFilter = MouseFilterEnum.Stop;

            _normal = MakeStyle(new Color("#241642"), new Color("#8054C7"), 2);
            _hover = MakeStyle(new Color("#38205F"), new Color("#55F6E8"), 2);
            _pressed = MakeStyle(new Color("#160D29"), new Color("#C9A7FF"), 3);
            _selectedStyle = MakeStyle(new Color("#49301D"), new Color("#FFD166"), 3);
            ApplyStyle();
            Refresh();
        }

        private static StyleBoxFlat MakeStyle(Color bg, Color border, int width)
        {
            var s = new StyleBoxFlat { BgColor = bg, BorderColor = border };
            s.SetBorderWidthAll(width);
            s.SetCornerRadiusAll(9);
            return s;
        }

        private void ApplyStyle() =>
            AddThemeStyleboxOverride("panel", _selected ? _selectedStyle : _normal);

        public override void _Notification(int what)
        {
            if (_normal == null || _selected) return;
            if (what == NotificationMouseEnter)
                AddThemeStyleboxOverride("panel", _hover);
            else if (what == NotificationMouseExit)
                AddThemeStyleboxOverride("panel", _normal);
        }

        public void Bind(InventorySlot slot) { _boundSlot = slot; Refresh(); }

        public void Refresh()
        {
            if (_icon == null || _quantity == null) return;

            bool isLocked = _boundSlot != null && _boundSlot.IsLocked;
            if (_lockIcon != null)
                _lockIcon.Visible = isLocked;

            bool hasItem = _boundSlot != null && !_boundSlot.IsEmpty;
            var def = hasItem ? _boundSlot.Stack.Item.Definition : null;
            _icon.Texture = def?.Icon;
            _icon.Visible = hasItem && def?.Icon != null;
            _quantity.Text = hasItem && _boundSlot.Stack.Quantity > 1
                ? _boundSlot.Stack.Quantity.ToString() : "";
        }

        public void SetSelected(bool selected) { _selected = selected; ApplyStyle(); }

        public override Variant _GetDragData(Vector2 atPosition)
        {
            if (_boundSlot == null || _boundSlot.IsEmpty) return default;
            var preview = new TextureRect
            {
                Texture = _boundSlot.Stack.Item.Definition.Icon,
                CustomMinimumSize = new Vector2(48, 48),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
            };
            SetDragPreview(preview);
            return SlotIndex;
        }

        public override bool _CanDropData(Vector2 atPosition, Variant data)
        {
            return data.VariantType == Variant.Type.Int &&
                   (int)data >= 0 && (int)data != SlotIndex &&
                   _boundSlot != null;
        }

        public override void _DropData(Vector2 atPosition, Variant data)
        {
            if (data.VariantType != Variant.Type.Int) return;
            int source = (int)data;
            if (source >= 0 && source != SlotIndex)
                SlotMoveRequested?.Invoke(source, SlotIndex);
        }

        public override void _GuiInput(InputEvent e)
{
    if (e is not InputEventMouseButton m || !m.Pressed || SlotIndex < 0)
        return;

    if (m.ButtonIndex == MouseButton.Left)
    {
        SlotSelected?.Invoke(SlotIndex);
    }
    else if (m.ButtonIndex == MouseButton.Right &&
             _boundSlot != null && !_boundSlot.IsEmpty)
    {
        int splitQuantity = _boundSlot.Stack.Quantity / 2;
        if (splitQuantity > 0)
        {
            SplitRequested?.Invoke(SlotIndex, splitQuantity);
            AcceptEvent();
        }
    }
}

}
}




