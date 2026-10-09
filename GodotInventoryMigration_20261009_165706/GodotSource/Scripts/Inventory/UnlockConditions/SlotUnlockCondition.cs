using Godot;

namespace InventorySystem.UnlockConditions
{
    // Extend this Resource to implement any game-specific unlock rule.
    [GlobalClass]
    public abstract partial class SlotUnlockCondition : Resource
    {
        public abstract bool IsMet(Node gameContext);
        public virtual string GetHint() => "Complete the required condition.";
    }
}
