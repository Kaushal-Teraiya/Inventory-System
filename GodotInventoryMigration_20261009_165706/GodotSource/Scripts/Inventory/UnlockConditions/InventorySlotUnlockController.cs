using Godot;
using InventorySystem.Inventory;

namespace InventorySystem.UnlockConditions
{
    [GlobalClass]
    public partial class InventorySlotUnlockController : Node
    {
        [Export]
        public Godot.Collections.Array<SlotUnlockDefinition> Definitions { get; set; } = new();

        [Export]
        public Node GameContext { get; set; }

        private _Inventory _inventory;

        [Signal]
        public delegate void SlotUnlockedEventHandler(int slotIndex);

        public void Initialize(_Inventory inventory)
        {
            _inventory = inventory;
            EvaluateConditions();
        }

        // Call after relevant game state changes, such as a quest completion
        // or currency update. No per-frame polling is performed.
        public void EvaluateConditions()
        {
            if (_inventory == null || GameContext == null || Definitions == null)
                return;

            foreach (var definition in Definitions)
            {
                if (definition == null ||
                    definition.SlotIndex < 0 ||
                    definition.SlotIndex >= _inventory.Capacity)
                    continue;

                var slot = _inventory.GetSlot(definition.SlotIndex);

                if (slot == null || !slot.IsLocked)
                    continue;

                if (definition.AreConditionsMet(GameContext) &&
                    _inventory.UnlockSlot(definition.SlotIndex))
                {
                    EmitSignal(SignalName.SlotUnlocked, definition.SlotIndex);
                }
            }
        }

        public bool UnlockSlot(int slotIndex)
        {
            if (_inventory == null || !_inventory.UnlockSlot(slotIndex))
                return false;

            EmitSignal(SignalName.SlotUnlocked, slotIndex);
            return true;
        }
    }
}
