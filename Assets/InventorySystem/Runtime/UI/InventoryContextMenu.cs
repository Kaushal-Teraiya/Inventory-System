using InventorySystem.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public sealed class InventoryContextMenu : MonoBehaviour
    {
        [SerializeField] private Button useButton;
        [SerializeField] private Button dropButton;
        [SerializeField] private Button splitButton;

        private InventoryUI inventoryUI;
        private int slotIndex = -1;

        public void Initialize(InventoryUI ui)
        {
            inventoryUI = ui;

            useButton?.onClick.RemoveAllListeners();
            dropButton?.onClick.RemoveAllListeners();
            splitButton?.onClick.RemoveAllListeners();

            useButton?.onClick.AddListener(Use);
            dropButton?.onClick.AddListener(Drop);
            splitButton?.onClick.AddListener(Split);

            gameObject.SetActive(false);
        }

        public void Open(int index)
        {
            if (inventoryUI == null)
                return;

            var slot = inventoryUI.Inventory.GetSlot(index);

            if (slot == null || slot.IsEmpty)
                return;

            slotIndex = index;

            if (Mouse.current != null)
                transform.position = Mouse.current.position.ReadValue();

            gameObject.SetActive(true);
        }

        public void Close()
        {
            slotIndex = -1;
            gameObject.SetActive(false);
        }

        private void Use()
        {
            if (inventoryUI == null || slotIndex < 0)
            {
                Close();
                return;
            }

            var inventory = inventoryUI.Inventory;
            var slot = inventory.GetSlot(slotIndex);

            if (slot == null || slot.IsEmpty)
            {
                Close();
                return;
            }

            var item = slot.Stack.Item;

            var result = inventory.RemoveItem(item, 1);

            if (result.Succeeded)
                Debug.Log($"USE: {item.Definition.DisplayName} from Slot {slotIndex}");
            else
                Debug.Log($"USE FAILED: {result.Failure}");

            Close();
        }

        private void Drop()
        {
            if (inventoryUI == null || slotIndex < 0)
            {
                Close();
                return;
            }

            var inventory = inventoryUI.Inventory;
            var slot = inventory.GetSlot(slotIndex);

            if (slot == null || slot.IsEmpty)
            {
                Close();
                return;
            }

            var item = slot.Stack.Item;
            int quantity = slot.Stack.Quantity;

            var result = inventory.RemoveItem(item, quantity);

            if (result.Succeeded)
                Debug.Log($"DROP: {quantity}x {item.Definition.DisplayName}");
            else
                Debug.Log($"DROP FAILED: {result.Failure}");

            Close();
        }

        private void Split()
        {
            if (inventoryUI == null || slotIndex < 0)
                return;

            var inventory = inventoryUI.Inventory;
            var slot = inventory.GetSlot(slotIndex);

            if (slot == null || slot.IsEmpty)
            {
                Close();
                return;
            }

            int quantity = slot.Stack.Quantity;

            if (quantity <= 1)
            {
                Debug.Log("SPLIT FAILED: Stack contains only one item.");
                Close();
                return;
            }

            int targetIndex = -1;

            for (int i = 0; i < inventory.Capacity; i++)
            {
                if (inventory.GetSlot(i).IsEmpty)
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex < 0)
            {
                Debug.Log("SPLIT FAILED: Inventory is full.");
                Close();
                return;
            }

            int splitQuantity = quantity / 2;

            var result = inventory.SplitStack(
                slotIndex,
                targetIndex,
                splitQuantity);

            if (result.Succeeded)
            {
                Debug.Log(
                    $"SPLIT: Slot {slotIndex} -> Slot {targetIndex}, Quantity {splitQuantity}");
            }
            else
            {
                Debug.Log(
                    $"SPLIT FAILED: {result.Failure}");
            }

            Close();
        }
    }
}


