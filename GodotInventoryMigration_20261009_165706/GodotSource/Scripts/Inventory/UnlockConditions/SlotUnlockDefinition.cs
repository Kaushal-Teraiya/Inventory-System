using Godot;

namespace InventorySystem.UnlockConditions
{
    [GlobalClass]
    public partial class SlotUnlockDefinition : Resource
    {
        [Export(PropertyHint.Range, "0,999,1")]
        public int SlotIndex { get; set; }

        [Export]
        public Godot.Collections.Array<SlotUnlockCondition> Conditions { get; set; } = new();

        [Export]
        public bool RequireAllConditions { get; set; } = true;

        [Export]
        public string LockedHint { get; set; } = "Meet the requirements to unlock this slot.";

        public bool AreConditionsMet(Node context)
        {
            if (Conditions == null || Conditions.Count == 0 || context == null)
                return false;

            bool anyValidCondition = false;

            foreach (var condition in Conditions)
            {
                if (condition == null)
                    continue;

                anyValidCondition = true;
                bool met = condition.IsMet(context);

                if (RequireAllConditions && !met)
                    return false;

                if (!RequireAllConditions && met)
                    return true;
            }

            return RequireAllConditions && anyValidCondition;
        }
    }
}
