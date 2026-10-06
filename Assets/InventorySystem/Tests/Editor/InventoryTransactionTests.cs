using System;
using System.Collections.Generic;
using InventorySystem.Inventory;
using NUnit.Framework;

namespace InventorySystem.Tests
{
    public class InventoryTransactionTests
    {
        [Test]
        public void Commit_ExecutesOperationsInOrder()
        {
            var transaction = new InventoryTransaction();
            var executionOrder = new List<int>();

            transaction.AddOperation(
                () =>
                {
                    executionOrder.Add(1);
                    return InventoryOperationResult.Success(1);
                },
                () => { });

            transaction.AddOperation(
                () =>
                {
                    executionOrder.Add(2);
                    return InventoryOperationResult.Success(1);
                },
                () => { });

            var result = transaction.Commit();

            Assert.That(result.Succeeded, Is.True);
            Assert.That(executionOrder, Is.EqualTo(new[] { 1, 2 }));
        }

        [Test]
        public void Commit_StopsAtFirstFailure()
        {
            var transaction = new InventoryTransaction();
            var executionCount = 0;

            transaction.AddOperation(
                () =>
                {
                    executionCount++;
                    return InventoryOperationResult.Success(1);
                },
                () => { });

            transaction.AddOperation(
                () =>
                {
                    executionCount++;
                    return InventoryOperationResult.Failed(
                        InventoryOperationResult.InventoryOperationFailure.InventoryFull);
                },
                () => { });

            transaction.AddOperation(
                () =>
                {
                    executionCount++;
                    return InventoryOperationResult.Success(1);
                },
                () => { });

            var result = transaction.Commit();

            Assert.That(result.Succeeded, Is.False);
            Assert.That(executionCount, Is.EqualTo(2));
            Assert.That(result.Failure,
                Is.EqualTo(InventoryOperationResult.InventoryOperationFailure.InventoryFull));
        }

        [Test]
        public void Commit_Failure_RollsBackSuccessfulOperationsInReverseOrder()
        {
            var transaction = new InventoryTransaction();
            var rollbackOrder = new List<int>();

            transaction.AddOperation(
                () => InventoryOperationResult.Success(1),
                () => rollbackOrder.Add(1));

            transaction.AddOperation(
                () => InventoryOperationResult.Success(1),
                () => rollbackOrder.Add(2));

            transaction.AddOperation(
                () => InventoryOperationResult.Failed(
                    InventoryOperationResult.InventoryOperationFailure.OperationNotAllowed),
                () => rollbackOrder.Add(3));

            var result = transaction.Commit();

            Assert.That(result.Succeeded, Is.False);
            Assert.That(rollbackOrder, Is.EqualTo(new[] { 2, 1 }));
        }

        [Test]
        public void OperationCount_TracksQueuedOperations()
        {
            var transaction = new InventoryTransaction();

            Assert.That(transaction.OperationCount, Is.EqualTo(0));

            transaction.AddOperation(
                () => InventoryOperationResult.Success(1),
                () => { });

            Assert.That(transaction.OperationCount, Is.EqualTo(1));
        }

        [Test]
        public void Clear_RemovesQueuedOperations()
        {
            var transaction = new InventoryTransaction();

            transaction.AddOperation(
                () => InventoryOperationResult.Success(1),
                () => { });

            transaction.Clear();

            Assert.That(transaction.OperationCount, Is.EqualTo(0));
        }

        [Test]
        public void AddOperation_NullExecute_Throws()
        {
            var transaction = new InventoryTransaction();

            Assert.Throws<ArgumentNullException>(() =>
                transaction.AddOperation(
                    null,
                    () => { }));
        }

        [Test]
        public void AddOperation_NullUndo_Throws()
        {
            var transaction = new InventoryTransaction();

            Assert.Throws<ArgumentNullException>(() =>
                transaction.AddOperation(
                    () => InventoryOperationResult.Success(1),
                    null));
        }

        [Test]
        public void EmptyTransaction_CommitsSuccessfully()
        {
            var transaction = new InventoryTransaction();

            var result = transaction.Commit();

            Assert.That(result.Succeeded, Is.True);
        }
    }
}




