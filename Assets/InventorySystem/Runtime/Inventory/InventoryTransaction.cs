using System;
using System.Collections.Generic;

namespace InventorySystem.Inventory
{
    public sealed class InventoryTransaction
    {
        private readonly List<TransactionOperation> operations = new();
        private bool isCommitting;

        public enum TransactionState
        {
            Pending,
            Committed,
            RolledBack
        }

        public int OperationCount => operations.Count;
        public TransactionState State { get; private set; } = TransactionState.Pending;

        public void AddOperation(
            Func<InventoryOperationResult> execute,
            Action undo)
        {
            if (isCommitting)
                throw new InvalidOperationException(
                    "Cannot add operations while the transaction is committing.");

            if (State != TransactionState.Pending)
                throw new InvalidOperationException(
                    "Cannot add operations to a completed transaction.");

            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            if (undo == null)
                throw new ArgumentNullException(nameof(undo));

            operations.Add(new TransactionOperation(execute, undo));
        }

        public InventoryOperationResult Commit()
        {
            if (isCommitting)
                throw new InvalidOperationException(
                    "Transaction is already committing.");

            if (State != TransactionState.Pending)
                throw new InvalidOperationException(
                    "Transaction has already completed.");

            isCommitting = true;

            var executed = new List<TransactionOperation>();

            try
            {
                foreach (var operation in operations)
                {
                    var result = operation.Execute();

                    if (!result.Succeeded)
                    {
                        Rollback(executed);
                        State = TransactionState.RolledBack;
                        operations.Clear();
                        return result;
                    }

                    executed.Add(operation);
                }

                State = TransactionState.Committed;
                operations.Clear();

                return InventoryOperationResult.Success(0);
            }
            catch
            {
                Rollback(executed);
                State = TransactionState.RolledBack;
                operations.Clear();
                throw;
            }
            finally
            {
                isCommitting = false;
            }
        }

        public void Clear()
        {
            if (isCommitting)
                throw new InvalidOperationException(
                    "Cannot clear a transaction while it is committing.");

            if (State != TransactionState.Pending)
                throw new InvalidOperationException(
                    "Cannot clear a completed transaction.");

            operations.Clear();
        }

        private static void Rollback(
            List<TransactionOperation> executed)
        {
            for (int i = executed.Count - 1; i >= 0; i--)
                executed[i].Undo();
        }

        private sealed class TransactionOperation
        {
            public Func<InventoryOperationResult> Execute { get; }
            public Action Undo { get; }

            public TransactionOperation(
                Func<InventoryOperationResult> execute,
                Action undo)
            {
                Execute = execute;
                Undo = undo;
            }
        }
    }
}
