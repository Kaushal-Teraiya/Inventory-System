namespace InventorySystem.Inventory
{
    public readonly struct InventoryOperationResult
    {
        public bool Succeeded { get; }
        public InventoryOperationFailure Failure { get; }
        public enum InventoryOperationFailure
        {
            None,
            InvalidItem,
            InventoryFull,
            InvalidSlot,
            InvalidQuantity,
            OperationNotAllowed
        }
        public int AffectedQuantity { get; }

        private InventoryOperationResult(bool succeeded, InventoryOperationFailure failure, int affectedQuantity)
        {
            Succeeded = succeeded;
            Failure = failure;
            AffectedQuantity = affectedQuantity;
        }

        public static InventoryOperationResult Success(int affectedQuantity)
        {
            return new InventoryOperationResult(true, InventoryOperationFailure.None, affectedQuantity);
        }

        public static InventoryOperationResult Failed(InventoryOperationFailure failure)
        {
            return new InventoryOperationResult(false, failure, 0);
        }
    }
}