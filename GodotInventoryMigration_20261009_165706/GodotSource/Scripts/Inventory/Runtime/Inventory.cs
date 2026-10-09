using System.Collections.Generic;
using InventorySystem.Domain;
using InventorySystem.Rules;

namespace InventorySystem.Inventory
{
    public sealed class _Inventory
    {
        private readonly List<InventorySlot> slots;

        public IReadOnlyList<InventorySlot> Slots => slots;

        public event System.Action<InventorySlot> SlotChanged;
        public event System.Action InventoryChanged;
        public int Capacity => slots.Count;
        public event System.Action<int> SlotUnlocked;

        public bool LockSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count)
                return false;

            var slot = slots[slotIndex];

            // Only empty slots can be locked.
            if (!slot.TryLock())
                return false;

            SlotChanged?.Invoke(slot);
            InventoryChanged?.Invoke();
            return true;
        }
        public bool UnlockSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count)
                return false;

            var slot = slots[slotIndex];
            if (slot.IsLocked)
                return false;
            if (!slot.Unlock())
                return false;

            SlotChanged?.Invoke(slot);
            InventoryChanged?.Invoke();
            SlotUnlocked?.Invoke(slotIndex);
            return true;
        }
        public float MaxWeight { get; }
        public float CurrentWeight { get; private set; }

        public _Inventory(int capacity, float maxWeight = float.MaxValue)
        {
            if (capacity < 0) throw new System.ArgumentOutOfRangeException(nameof(capacity), capacity, "Inventory capacity cannot be negative.");
            if (maxWeight <= 0) throw new System.ArgumentOutOfRangeException(nameof(maxWeight), maxWeight, "Maximum inventory weight must be greater than zero.");
            MaxWeight = maxWeight;
             
            slots = new List<InventorySlot>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                slots.Add(new InventorySlot(i));
            }
        }

        public InventoryOperationResult AddItem(
            ItemInstance item,
            int quantity)
        {
            if (item == null)
            {
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.InvalidItem);
            }

            if (quantity <= 0)
            {
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.InvalidQuantity);
            }

            if (CurrentWeight + item.Definition.Weight * quantity > MaxWeight)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.InventoryFull);

            int remainingCapacity = CalculateAvailableCapacity(item);

            if (remainingCapacity < quantity)
            {
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.InventoryFull);
            }

            int remaining = quantity;

            foreach (var slot in slots)
            {
                if (slot.IsEmpty)
                {
                    continue;
                }

                if (!InventoryRules.AreItemsStackCompatible(
                        slot.Stack.Item,
                        item))
                {
                    continue;
                }

                int stackCapacity =
                    InventoryRules.GetRemainingStackCapacity(slot.Stack);

                int amountToAdd =
                    remaining < stackCapacity
                        ? remaining
                        : stackCapacity;

                if (amountToAdd > 0)
                {
                    slot.Stack.AddQuantity(amountToAdd);
                    remaining -= amountToAdd;
                    NotifySlotChanged(slot);
                }

                if (remaining == 0)
                {
                    break;
                }
            }

            foreach (var slot in slots)
            {
                if (remaining == 0)
                {
                    break;
                }

                if (!slot.IsEmpty)
                {
                    continue;
                }

                int amountToAdd =
                    remaining < item.Definition.MaxStackSize
                        ? remaining
                        : item.Definition.MaxStackSize;

                var stack = new ItemStack(item, amountToAdd);

                slot.SetStack(stack);
                remaining -= amountToAdd;
                NotifySlotChanged(slot);
            }
            return InventoryOperationResult.Success(quantity);
        }

        private int CalculateAvailableCapacity(ItemInstance item)
        {
            int capacity = 0;

            foreach (var slot in slots)
            {
                if (slot.IsEmpty)
                {
                    capacity += item.Definition.MaxStackSize;
                    continue;
                }

                if (InventoryRules.AreItemsStackCompatible(
                        slot.Stack.Item,
                        item))
                {
                    capacity +=
                        InventoryRules.GetRemainingStackCapacity(
                            slot.Stack);
                }
            }

            return capacity;
        }

        public InventoryOperationResult RemoveItem(ItemInstance item, int quantity)
        {
            if (item == null) return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.InvalidItem);
            if (quantity <= 0) return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.InvalidQuantity);

            int availableQuantity = CalculateAvailableQuantity(item);
            if (availableQuantity < quantity) return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            int remaining = quantity;

            foreach (var slot in slots)
            {
                if (slot.IsLocked || slot.IsEmpty) continue;
                if (!InventoryRules.AreItemsStackCompatible(slot.Stack.Item, item)) continue;

                int amountToRemove = remaining < slot.Stack.Quantity ? remaining : slot.Stack.Quantity;

                slot.Stack.TryRemoveQuantity(amountToRemove);
                remaining -= amountToRemove;

                if (slot.Stack.Quantity == 0) slot.Clear();

                NotifySlotChanged(slot);
                if (remaining == 0) break;
            }

            return InventoryOperationResult.Success(quantity);
        }


        public InventoryOperationResult SplitStack(int sourceIndex, int targetIndex, int quantity)
        {
            if (sourceIndex < 0 || sourceIndex >= slots.Count || targetIndex < 0 || targetIndex >= slots.Count)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.InvalidSlot);

            if (sourceIndex == targetIndex)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            if (quantity <= 0)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.InvalidQuantity);

            var source = slots[sourceIndex];
            var target = slots[targetIndex];
            if (source.IsLocked || target.IsLocked)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);
            if (source.IsLocked || target.IsLocked)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            if (source.IsEmpty || !target.IsEmpty)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            if (quantity >= source.Stack.Quantity)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.InvalidQuantity);

            source.Stack.TryRemoveQuantity(quantity);
            target.SetStack(new ItemStack(source.Stack.Item, quantity));

            NotifySlotChanged(source);
            NotifySlotChanged(target);

            return InventoryOperationResult.Success(quantity);
        }
        public InventoryOperationResult TransferTo(_Inventory targetInventory, ItemInstance item, int quantity)
        {
            if (targetInventory == null || item == null)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.InvalidItem);

            if (quantity <= 0)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.InvalidQuantity);

            var available = CalculateAvailableQuantity(item);
            if (available < quantity)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            var result = targetInventory.AddItem(item, quantity);
            if (!result.Succeeded)
                return result;

            RemoveItem(item, quantity);
            return result;
        }
        private void NotifySlotChanged(InventorySlot slot)
        {
            SlotChanged?.Invoke(slot);
            InventoryChanged?.Invoke();
        }

        private void NotifyInventoryChanged()
        {
            InventoryChanged?.Invoke();
        }

        public InventoryOperationResult TransferToTransactional(
            _Inventory target,
            ItemInstance item,
            int quantity)
        {
            if (target == null)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            if (item == null)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.InvalidItem);

            if (quantity <= 0)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.InvalidQuantity);

            int sourceQuantityBefore = GetItemQuantity(item);

            if (sourceQuantityBefore < quantity)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.InvalidQuantity);

            var transaction = new InventoryTransaction();

            transaction.AddOperation(
                () => RemoveItem(item, quantity),
                () => AddItem(item, quantity));

            transaction.AddOperation(
                () => target.AddItem(item, quantity),
                () => target.RemoveItem(item, quantity));

            return transaction.Commit();
        }
        public InventorySnapshot CreateSnapshot()
        {
            var slots = new InventorySnapshot.SnapshotSlot[Capacity];

            for (int i = 0; i < Capacity; i++)
            {
                var slot = Slots[i];

                slots[i] = slot.IsEmpty
                    ? new InventorySnapshot.SnapshotSlot(i, null, 0)
                    : new InventorySnapshot.SnapshotSlot(
                        i,
                        slot.Stack.Item,
                        slot.Stack.Quantity);
            }

            return new InventorySnapshot(slots);
        }

        public void RestoreSnapshot(InventorySnapshot snapshot)
        {
            if (snapshot == null)
                throw new System.ArgumentNullException(nameof(snapshot));

            if (snapshot.Slots == null ||
                snapshot.Slots.Length != Capacity)
                throw new System.ArgumentException(
                    "Snapshot capacity does not match inventory capacity.",
                    nameof(snapshot));

            Clear();

            foreach (var savedSlot in snapshot.Slots)
            {
                if (savedSlot.Item == null)
                    continue;

                var result = AddItemToSlot(
                    savedSlot.Index,
                    savedSlot.Item,
                    savedSlot.Quantity);

                if (!result.Succeeded)
                    throw new System.InvalidOperationException(
                        "Failed to restore inventory snapshot.");
            }
        }
        public void Clear()
        {
            bool changed = false;

            foreach (var slot in slots)
            {
                if (slot.IsEmpty)
                    continue;

                slot.Clear();
                SlotChanged?.Invoke(slot);
                changed = true;
            }

            CurrentWeight = 0f;

            if (changed)
                NotifyInventoryChanged();
        }
        public InventoryOperationResult AddItemToSlot(int slotIndex, ItemInstance item, int quantity)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.InvalidSlot);

            if (item == null)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.InvalidItem);

            if (quantity <= 0)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.InvalidQuantity);

            if (CurrentWeight + item.Definition.Weight * quantity > MaxWeight)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.InventoryFull);

            var slot = slots[slotIndex];
            if (slot.IsLocked)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            if (slot.IsEmpty)
            {
                if (quantity > item.Definition.MaxStackSize)
                    return InventoryOperationResult.Failed(
                        InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

                slot.SetStack(new ItemStack(item, quantity));
                CurrentWeight += item.Definition.Weight * quantity;
                NotifySlotChanged(slot);

                return InventoryOperationResult.Success(quantity);
            }

            if (!InventoryRules.AreItemsStackCompatible(slot.Stack.Item, item))
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            int capacity = InventoryRules.GetRemainingStackCapacity(slot.Stack);

            if (quantity > capacity)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            slot.Stack.AddQuantity(quantity);
            CurrentWeight += item.Definition.Weight * quantity;
            NotifySlotChanged(slot);

            return InventoryOperationResult.Success(quantity);
        }
        public InventoryOperationResult MoveItem(int sourceIndex, int targetIndex)
        {
            if (sourceIndex < 0 || sourceIndex >= slots.Count ||
                targetIndex < 0 || targetIndex >= slots.Count)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.InvalidSlot);

            if (sourceIndex == targetIndex)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            var source = slots[sourceIndex];
            var target = slots[targetIndex];
            if (source.IsLocked || target.IsLocked)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);
            if (source.IsLocked || target.IsLocked)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            if (source.IsEmpty)
                return InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            if (target.IsEmpty)
            {
                target.SetStack(source.Stack);
                source.Clear();

                NotifySlotChanged(source);
                NotifySlotChanged(target);

                return InventoryOperationResult.Success(target.Stack.Quantity);
            }

            if (InventoryRules.AreItemsStackCompatible(source.Stack.Item, target.Stack.Item))
            {
                int capacity = InventoryRules.GetRemainingStackCapacity(target.Stack);

                if (capacity <= 0)
                    return InventoryOperationResult.Failed(
                        InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

                int amount = source.Stack.Quantity < capacity
                    ? source.Stack.Quantity
                    : capacity;

                target.Stack.AddQuantity(amount);
                source.Stack.TryRemoveQuantity(amount);

                if (source.Stack.Quantity == 0)
                    source.Clear();

                NotifySlotChanged(source);
                NotifySlotChanged(target);

                return InventoryOperationResult.Success(amount);
            }

            var sourceStack = source.Stack;
            source.SetStack(target.Stack);
            target.SetStack(sourceStack);

            NotifySlotChanged(source);
            NotifySlotChanged(target);

            return InventoryOperationResult.Success(sourceStack.Quantity);
        }
        public InventoryOperationResult RemoveFromSlot(int slotIndex, int quantity)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.InvalidSlot);

            if (quantity <= 0)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.InvalidQuantity);

            var slot = slots[slotIndex];
            if (slot.IsLocked)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            if (slot.IsEmpty || slot.Stack.Quantity < quantity)
                return InventoryOperationResult.Failed(InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed);

            var item = slot.Stack.Item;

            slot.Stack.TryRemoveQuantity(quantity);

            if (slot.Stack.Quantity == 0)
                slot.Clear();

            CurrentWeight -= item.Definition.Weight * quantity;

            NotifySlotChanged(slot);

            return InventoryOperationResult.Success(quantity);
        }
        public InventorySlot GetSlot(int index)
        {
            if (index < 0 || index >= slots.Count)
                return null;

            return slots[index];
        }
        public bool IsEmpty => GetTotalItemCount() == 0;

        public bool IsFull => GetTotalItemCount() >= Capacity;

        public int GetTotalItemCount()
        {
            int count = 0;

            foreach (var slot in slots)
            {
                if (!slot.IsEmpty)
                    count++;
            }

            return count;
        }
        public bool HasItem(ItemInstance item, int quantity = 1)
        {
            return item != null && quantity > 0 && CalculateAvailableQuantity(item) >= quantity;
        }

        public int GetItemQuantity(ItemInstance item)
        {
            return item == null ? 0 : CalculateAvailableQuantity(item);
        }

        public int FindItemSlot(ItemInstance item)
        {
            if (item == null) return -1;

            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && InventoryRules.AreItemsStackCompatible(slot.Stack.Item, item))
                    return slot.Index;
            }

            return -1;
        }
        private int CalculateAvailableQuantity(ItemInstance item)
        {
            int quantity = 0;

            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && InventoryRules.AreItemsStackCompatible(slot.Stack.Item, item))
                    quantity += slot.Stack.Quantity;
            }

            return quantity;
        }
    }
}







