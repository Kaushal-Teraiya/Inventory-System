using InventorySystem.Domain;
using InventorySystem.Inventory;
using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem.UI
{
    public sealed class InventoryUI : MonoBehaviour
    {
        [SerializeField] private int inventoryCapacity = 24;
        [SerializeField] private ItemDefinition testItem;
        [SerializeField] private int testQuantity = 5;

        private _Inventory inventory;
        private InventorySlotUI[] slots;
        private InventoryUIMouseInput mouseInput;
        private int selectedSlotIndex = -1;

        public _Inventory Inventory => inventory;
        public int SelectedSlotIndex => selectedSlotIndex;

        public InventorySlotUI SelectedSlotUI =>
            selectedSlotIndex >= 0 && selectedSlotIndex < slots.Length
                ? slots[selectedSlotIndex]
                : null;

        private void Awake()
        {
            inventory = new _Inventory(inventoryCapacity);

            var slotTransforms =
                GetComponentsInChildren<Transform>(true);

            var foundSlots =
                new System.Collections.Generic.List<InventorySlotUI>();

            foreach (var transform in slotTransforms)
            {
                if (!transform.name.StartsWith("Slot_"))
                    continue;

                var slotUI =
                    transform.GetComponent<InventorySlotUI>();

                if (slotUI == null)
                    slotUI =
                        transform.gameObject.AddComponent<InventorySlotUI>();

                foundSlots.Add(slotUI);
            }

            slots = foundSlots.ToArray();

            mouseInput = GetComponent<InventoryUIMouseInput>();

            if (mouseInput == null)
                mouseInput = gameObject.AddComponent<InventoryUIMouseInput>();

            Debug.Log(
                $"InventoryUI initialized. " +
                $"Inventory capacity={inventory.Capacity}, " +
                $"UI slots found={slots.Length}");

            if (testItem != null)
            {
                inventory.AddItem(
                    new ItemInstance(testItem),
                    testQuantity);
            }
        }

        private void OnEnable()
        {
            if (inventory == null)
                return;

            inventory.SlotChanged += HandleSlotChanged;
            RefreshAll();
        }

        private void OnDisable()
        {
            if (inventory != null)
                inventory.SlotChanged -= HandleSlotChanged;
        }


        public void SelectSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length)
                return;

            if (selectedSlotIndex >= 0 &&
                selectedSlotIndex < slots.Length)
            {
                slots[selectedSlotIndex].SetSelected(false);
            }

            selectedSlotIndex = slotIndex;
            slots[selectedSlotIndex].SetSelected(true);
        }
        private void HandleSlotChanged(InventorySlot changedSlot)
        {
            if (changedSlot == null)
                return;

            if (changedSlot.Index < 0 ||
                changedSlot.Index >= slots.Length)
                return;

            Debug.Log(
                $"Refreshing UI slot {changedSlot.Index}");

            slots[changedSlot.Index].Refresh();
        }

        private void RefreshAll()
        {
            if (inventory == null)
                return;

            int count = Mathf.Min(
                inventory.Capacity,
                slots.Length);

            Debug.Log(
                $"Binding {count} inventory slots.");

            for (int i = 0; i < count; i++)
                slots[i].Bind(
                    inventory.GetSlot(i));
        }
    }
}



