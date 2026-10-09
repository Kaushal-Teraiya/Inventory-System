using Godot;
using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using InventorySystem.Player;

namespace InventorySystem.UI
{
    public partial class InventoryUI : Control
    {
        [Export] public int InventoryCapacity { get; set; } = 20;
        [Export] public float MaximumWeight { get; set; } = 100f;
        [Export] public Godot.Collections.Array<ItemDefinition> StartingItems { get; set; } = new();

        private _Inventory _inventory;
        private InventorySlotUI[] _slots = System.Array.Empty<InventorySlotUI>();
        private int _selectedSlotIndex = -1;
        private int _pendingSplitSource = -1;
        private int _pendingSplitQuantity;

        private Label _capacityLabel;
        private TextureRect _detailIcon;
        private Label _itemName;
        private Label _itemDescription;
        private Button _useButton;
        private Button _dropButton;
        private PlayerHealth _playerHealth;
        private CanvasLayer _healthHud;
        private ProgressBar _healthBar;
        private Label _healthLabel;
        private Label _healthMessage;

        public _Inventory Inventory => _inventory;
        public int SelectedSlotIndex => _selectedSlotIndex;

        public override void _Ready()
        {
            _inventory = new _Inventory(InventoryCapacity, MaximumWeight);
            CreateHealthHud();

            var grid = GetNode<GridContainer>(
                "MainPanel/MarginContainer/MainLayout/Content/InventorySection/InventoryGrid"
            );

            _capacityLabel = GetNode<Label>(
                "MainPanel/MarginContainer/MainLayout/Content/InventorySection/CapacityLabel"
            );
            _detailIcon = GetNode<TextureRect>(
                "MainPanel/MarginContainer/MainLayout/Content/ItemDetails/DetailsLayout/DetailIcon"
            );
            _itemName = GetNode<Label>(
                "MainPanel/MarginContainer/MainLayout/Content/ItemDetails/DetailsLayout/ItemName"
            );
            _itemDescription = GetNode<Label>(
                "MainPanel/MarginContainer/MainLayout/Content/ItemDetails/DetailsLayout/ItemDescription"
            );
            _useButton = GetNode<Button>(
                "MainPanel/MarginContainer/MainLayout/Content/ItemDetails/DetailsLayout/ActionButtons/UseButton"
            );
            _dropButton = GetNode<Button>(
                "MainPanel/MarginContainer/MainLayout/Content/ItemDetails/DetailsLayout/ActionButtons/DropButton"
            );

            _slots = new InventorySlotUI[grid.GetChildCount()];

            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i] = grid.GetChild(i) as InventorySlotUI;

                if (_slots[i] == null)
                {
                    GD.PushError($"Inventory slot {i} is missing InventorySlotUI.");
                    return;
                }

                _slots[i].SlotSelected += SelectSlot;
                _slots[i].SlotMoveRequested += MoveSlot;
                _slots[i].SplitRequested += BeginSplit;
                _slots[i].Bind(_inventory.GetSlot(i));
            }

            _inventory.SlotChanged += HandleSlotChanged;
            _inventory.InventoryChanged += RefreshCapacity;

            GetNode<Button>("MainPanel/MarginContainer/MainLayout/Header/CloseButton")
                .Pressed += CloseInventory;
            GetNode<Button>("MainPanel/MarginContainer/MainLayout/Footer/CloseButtonFooter")
                .Pressed += CloseInventory;

            _dropButton.Pressed += DropSelectedStack;
            _useButton.Pressed += UseOneItem;
            _useButton.TooltipText = "Use one item from this stack.";

            GetNode<Button>("MainPanel/MarginContainer/MainLayout/Footer/SortButton")
                .Disabled = true;

            foreach (var definition in StartingItems)
            {
                if (definition == null || !definition.IsValid)
                    continue;

                var result = _inventory.AddItem(new ItemInstance(definition), 10);
                if (!result.Succeeded)
                    GD.PushWarning($"Could not add starting item: {definition.DisplayName}");
            }

            ApplyVisualTheme();

            RandomlyLockOneEmptySlot();
            RefreshCapacity();
            RefreshDetails();
        }
        private static StyleBoxFlat CreatePanelStyle(Color background, Color border, int borderWidth, int radius)
        {
            var style = new StyleBoxFlat
            {
                BgColor = background,
                BorderColor = border,
                ContentMarginLeft = 12,
                ContentMarginRight = 12,
                ContentMarginTop = 10,
                ContentMarginBottom = 10
            };
            style.SetBorderWidthAll(borderWidth);
            style.SetCornerRadiusAll(radius);
            return style;
        }

        private void ApplyVisualTheme()
        {
            var panel = GetNode<PanelContainer>("MainPanel");
            panel.AddThemeStyleboxOverride(
                "panel",
                CreatePanelStyle(new Color("#130D25"), new Color("#B56BFF"), 2, 12));

            var details = GetNode<PanelContainer>(
                "MainPanel/MarginContainer/MainLayout/Content/ItemDetails");
            details.AddThemeStyleboxOverride(
                "panel",
                CreatePanelStyle(new Color("#211536"), new Color("#754BC4"), 1, 9));

            var margin = GetNode<MarginContainer>("MainPanel/MarginContainer");
            margin.AddThemeConstantOverride("margin_left", 18);
            margin.AddThemeConstantOverride("margin_right", 18);
            margin.AddThemeConstantOverride("margin_top", 16);
            margin.AddThemeConstantOverride("margin_bottom", 14);

            var layout = GetNode<VBoxContainer>("MainPanel/MarginContainer/MainLayout");
            layout.AddThemeConstantOverride("separation", 16);

            var content = GetNode<HBoxContainer>(
                "MainPanel/MarginContainer/MainLayout/Content");
            content.AddThemeConstantOverride("separation", 20);

            var grid = GetNode<GridContainer>(
                "MainPanel/MarginContainer/MainLayout/Content/InventorySection/InventoryGrid");
            grid.AddThemeConstantOverride("h_separation", 9);
            grid.AddThemeConstantOverride("v_separation", 9);

            var title = GetNode<Label>("MainPanel/MarginContainer/MainLayout/Header/Title");
            title.AddThemeColorOverride("font_color", new Color("#FFE08A"));
            title.AddThemeFontSizeOverride("font_size", 28);

            _capacityLabel.AddThemeColorOverride("font_color", new Color("#67F5E5"));
            _capacityLabel.AddThemeFontSizeOverride("font_size", 15);

            _itemName.AddThemeColorOverride("font_color", new Color("#FFE08A"));
            _itemName.AddThemeFontSizeOverride("font_size", 21);

            _itemDescription.AddThemeColorOverride("font_color", new Color("#E2D8FF"));
            _itemDescription.AddThemeFontSizeOverride("font_size", 15);

            foreach (var path in new[]
            {
                "MainPanel/MarginContainer/MainLayout/Header/CloseButton",
                "MainPanel/MarginContainer/MainLayout/Footer/CloseButtonFooter",
                "MainPanel/MarginContainer/MainLayout/Footer/SortButton",
                "MainPanel/MarginContainer/MainLayout/Content/ItemDetails/DetailsLayout/ActionButtons/UseButton",
                "MainPanel/MarginContainer/MainLayout/Content/ItemDetails/DetailsLayout/ActionButtons/DropButton"
            })
            {
                var button = GetNode<Button>(path);
                button.AddThemeStyleboxOverride(
                    "normal",
                    CreatePanelStyle(new Color("#402464"), new Color("#9268DB"), 1, 6));
                button.AddThemeStyleboxOverride(
                    "hover",
                    CreatePanelStyle(new Color("#633A99"), new Color("#67F5E5"), 1, 6));
                button.AddThemeStyleboxOverride(
                    "pressed",
                    CreatePanelStyle(new Color("#241538"), new Color("#FFE08A"), 2, 6));
                button.AddThemeColorOverride("font_color", new Color("#edf1f7"));
                button.AddThemeColorOverride("font_hover_color", new Color("#ffe2a0"));
                button.AddThemeFontSizeOverride("font_size", 14);
                button.CustomMinimumSize = new Vector2(72, 36);
            }
        }

        public override void _ExitTree()
        {
            if (_playerHealth != null)
                _playerHealth.HealthChanged -= RefreshHealthHud;

            if (GodotObject.IsInstanceValid(_healthHud))
                _healthHud.QueueFree();

            if (_inventory != null)
            {
                _inventory.SlotChanged -= HandleSlotChanged;
                _inventory.InventoryChanged -= RefreshCapacity;
            }

            foreach (var slot in _slots)
            {
                if (slot == null)
                    continue;

                slot.SlotSelected -= SelectSlot;
                slot.SlotMoveRequested -= MoveSlot;
                slot.SplitRequested -= BeginSplit;
            }
        }

        public void SelectSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Length)
                return;

            if (_pendingSplitSource >= 0)
            {
                int source = _pendingSplitSource;
                int quantity = _pendingSplitQuantity;

                _pendingSplitSource = -1;
                _pendingSplitQuantity = 0;

                if (slotIndex != source)
                {
                    GD.Print($"Attempting split: source={source}, destination={slotIndex}, quantity={quantity}");

                    var result = _inventory.SplitStack(source, slotIndex, quantity);

                    GD.Print($"Split result: success={result.Succeeded}");

                    if (!result.Succeeded)
                        GD.PushWarning("Split failed. Choose an empty destination slot and split less than the source quantity.");
                }
                else
                {
                    GD.Print("Split cancelled.");
                }
            }

            if (_selectedSlotIndex >= 0 && _selectedSlotIndex < _slots.Length)
                _slots[_selectedSlotIndex].SetSelected(false);

            _selectedSlotIndex = slotIndex;
            _slots[_selectedSlotIndex].SetSelected(true);
            RefreshDetails();
        }

        private void BeginSplit(int sourceIndex, int quantity)
        {
            if (quantity <= 0)
            {
                GD.PushWarning("This stack cannot be split.");
                return;
            }

            _pendingSplitSource = sourceIndex;
            _pendingSplitQuantity = quantity;
            GD.Print($"Split armed: choose a destination slot for {quantity} items.");
        }

        private void MoveSlot(int sourceIndex, int targetIndex)
        {
            var result = _inventory.MoveItem(sourceIndex, targetIndex);
            if (!result.Succeeded)
                GD.PushWarning("Move failed. Check the destination slot.");
        }

        private void HandleSlotChanged(InventorySlot slot)
        {
            if (slot == null || slot.Index < 0 || slot.Index >= _slots.Length)
                return;

            _slots[slot.Index].Refresh();

            if (slot.Index == _selectedSlotIndex)
                RefreshDetails();
        }

        private void RefreshCapacity()
        {
            if (_capacityLabel != null && _inventory != null)
                _capacityLabel.Text =
                    $"Inventory: {_inventory.GetTotalItemCount()} / {_inventory.Capacity}";
        }

        private void RefreshDetails()
        {
            var slot = _inventory?.GetSlot(_selectedSlotIndex);
            bool hasItem = slot != null && !slot.IsEmpty;

            _detailIcon.Texture = hasItem ? slot.Stack.Item.Definition.Icon : null;
            _detailIcon.Visible = hasItem && slot.Stack.Item.Definition.Icon != null;
            _itemName.Text = hasItem ? slot.Stack.Item.Definition.DisplayName : "Select an item";

            _itemDescription.Text = hasItem
                ? $"Quantity: {slot.Stack.Quantity}\nWeight each: {slot.Stack.Item.Definition.Weight:0.##}\nStack limit: {slot.Stack.Item.Definition.MaxStackSize}"
                : "Select an occupied slot to see item details.";

            _useButton.Disabled = !hasItem;
            _dropButton.Disabled = !hasItem;
        }

        private readonly ItemUseService _itemUseService = new ItemUseService();

        private void UseOneItem()
        {
            if (_inventory == null || _playerHealth == null || _selectedSlotIndex < 0)
                return;

            bool used = _itemUseService.TryUse(
                _inventory, _selectedSlotIndex, _playerHealth, out string message);

            ShowHealthMessage(message);
            if (used)
                GD.Print($"ITEM USED: {message} Health = {_playerHealth.CurrentHealth}/{_playerHealth.MaxHealth}");
        }

        private void DropSelectedStack()
        {
            if (_inventory == null || _selectedSlotIndex < 0)
                return;

            var slot = _inventory.GetSlot(_selectedSlotIndex);
            if (slot == null || slot.IsEmpty)
                return;

            int quantity = slot.Stack.Quantity;
            var result = _inventory.RemoveFromSlot(_selectedSlotIndex, quantity);

            if (!result.Succeeded)
                GD.PushWarning($"Could not drop stack from slot {_selectedSlotIndex}.");
        }

        private void CloseInventory()
        {
            Hide();
        }

        private void RandomlyLockOneEmptySlot()
        {
            var candidates = new System.Collections.Generic.List<int>();

            for (int i = 0; i < _inventory.Capacity; i++)
            {
                var slot = _inventory.GetSlot(i);
                if (slot != null && !slot.IsLocked && slot.IsEmpty)
                    candidates.Add(i);
            }

            if (candidates.Count == 0)
            {
                GD.PushWarning("No empty slot is available to lock.");
                return;
            }

            int chosenIndex = candidates[System.Random.Shared.Next(candidates.Count)];

            if (_inventory.LockSlot(chosenIndex))
                GD.Print($"Randomly locked inventory slot {chosenIndex}.");
            else
                GD.PushWarning($"Could not lock inventory slot {chosenIndex}.");
        }

        private void CreateHealthHud()
        {
            _playerHealth = new PlayerHealth(100);
            _playerHealth.HealthChanged += RefreshHealthHud;

            _healthHud = new CanvasLayer { Name = "PlayerHealthHUD", Layer = 10 };
            GetTree().Root.CallDeferred(Node.MethodName.AddChild, _healthHud);

            var panel = new PanelContainer
            {
                Name = "HealthPanel",
                Position = new Vector2(20, 20),
                CustomMinimumSize = new Vector2(300, 150)
            };
            _healthHud.AddChild(panel);

            var layout = new VBoxContainer();
            layout.AddThemeConstantOverride("separation", 6);
            panel.AddChild(layout);

            var title = new Label { Text = "PLAYER HEALTH" };
            layout.AddChild(title);

            _healthLabel = new Label();
            layout.AddChild(_healthLabel);

            _healthBar = new ProgressBar
            {
                MinValue = 0,
                MaxValue = _playerHealth.MaxHealth,
                ShowPercentage = false,
                CustomMinimumSize = new Vector2(260, 24)
            };
            layout.AddChild(_healthBar);

            var damageButton = new Button
            {
                Text = "Take 25 Damage (TEST)",
                CustomMinimumSize = new Vector2(260, 32)
            };
            damageButton.Pressed += () =>
            {
                int damage = _playerHealth.TakeDamage(25);
                ShowHealthMessage(damage > 0
                    ? $"-{damage} HP taken"
                    : "Player is already at 0 HP.");
                GD.Print($"DAMAGE TEST: Health = {_playerHealth.CurrentHealth}/{_playerHealth.MaxHealth}");
            };
            layout.AddChild(damageButton);

            _healthMessage = new Label { Text = "Use a healing item to restore HP." };
            _healthMessage.CustomMinimumSize = new Vector2(260, 28);
            layout.AddChild(_healthMessage);

            RefreshHealthHud();
        }

        private void RefreshHealthHud()
        {
            if (_playerHealth == null || _healthBar == null || _healthLabel == null)
                return;

            _healthBar.Value = _playerHealth.CurrentHealth;
            _healthLabel.Text =
                $"HP: {_playerHealth.CurrentHealth} / {_playerHealth.MaxHealth}";
        }

        private void ShowHealthMessage(string message)
        {
            if (_healthMessage != null)
                _healthMessage.Text = message;
        }
    }
}









