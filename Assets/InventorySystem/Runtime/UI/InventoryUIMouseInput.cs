using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public sealed class InventoryUIMouseInput : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private InventoryContextMenu contextMenu;

        private InventoryUI inventoryUI;
        private InventorySlotUI hoveredSlot;
        private InventorySlotUI draggedSlot;

        private void Awake()
        {
            inventoryUI = GetComponent<InventoryUI>();

            if (raycaster == null)
                raycaster = GetComponent<GraphicRaycaster>();

            if (raycaster == null)
                raycaster = GetComponentInParent<GraphicRaycaster>();

            if (contextMenu == null)
                contextMenu =
                    GetComponentInChildren<InventoryContextMenu>(true);

            if (contextMenu != null && inventoryUI != null)
                contextMenu.Initialize(inventoryUI);
        }

        private void Update()
        {
            if (raycaster == null ||
                Mouse.current == null ||
                EventSystem.current == null)
                return;

            // Menu is open: handle only menu input.
            if (contextMenu != null &&
                contextMenu.gameObject.activeSelf &&
                contextMenu.SlotIndex >= 0)
            {
                HandleContextMenuInput();
                return;
            }

            UpdateHoveredSlot();

            // Right click opens menu.
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                OpenContextMenu();
                return;
            }

            // Left mouse controls dragging.
            if (Mouse.current.leftButton.wasPressedThisFrame)
                StartDrag();

            if (Mouse.current.leftButton.wasReleasedThisFrame)
                EndDrag();
        }

        private List<RaycastResult> RaycastMouse()
        {
            var pointer =
                new PointerEventData(EventSystem.current)
                {
                    position =
                        Mouse.current.position.ReadValue()
                };

            var results =
                new List<RaycastResult>();

            raycaster.Raycast(pointer, results);

            return results;
        }

        private void UpdateHoveredSlot()
        {
            var results = RaycastMouse();

            InventorySlotUI slot = null;

            foreach (var result in results)
            {
                slot =
                    result.gameObject
                        .GetComponentInParent<InventorySlotUI>();

                if (slot != null)
                    break;
            }

            if (slot == hoveredSlot)
                return;

            if (hoveredSlot != null &&
                hoveredSlot != draggedSlot)
            {
                hoveredSlot.SetSelected(false);
            }

            hoveredSlot = slot;

            if (hoveredSlot != null &&
                hoveredSlot != draggedSlot)
            {
                hoveredSlot.SetSelected(true);
            }
        }

        private void StartDrag()
        {
            if (hoveredSlot == null ||
                hoveredSlot.BoundSlot == null ||
                hoveredSlot.BoundSlot.IsEmpty)
                return;

            draggedSlot = hoveredSlot;

            Debug.Log(
                $"Started dragging Slot {draggedSlot.BoundSlot.Index}");
        }

        private void EndDrag()
        {
            if (draggedSlot == null)
                return;

            var source = draggedSlot.BoundSlot;
            var target = hoveredSlot?.BoundSlot;

            if (source != null &&
                target != null &&
                source.Index != target.Index)
            {
                var result =
                    inventoryUI.Inventory.MoveItem(
                        source.Index,
                        target.Index);

                if (result.Succeeded)
                {
                    Debug.Log(
                        $"Moved Slot {source.Index} -> Slot {target.Index}");

                    inventoryUI.SelectSlot(target.Index);
                }
            }

            draggedSlot = null;
        }

        private void OpenContextMenu()
        {
            if (contextMenu == null ||
                hoveredSlot == null ||
                hoveredSlot.BoundSlot == null ||
                hoveredSlot.BoundSlot.IsEmpty)
                return;

            contextMenu.Open(
                hoveredSlot.BoundSlot.Index);
        }

        private void HandleContextMenuInput()
        {
            if (Keyboard.current != null &&
                Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                contextMenu.Close();
                return;
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;

            var results = RaycastMouse();

            foreach (var result in results)
            {
                var button =
                    result.gameObject
                        .GetComponentInParent<Button>();

                if (button == null)
                    continue;

                Debug.Log(
                    $"Context button clicked: {button.name}");

                var menu = result.gameObject.GetComponentInParent<InventoryContextMenu>();

if (menu == null)
    return;

if (button.name.Equals("Use", System.StringComparison.OrdinalIgnoreCase))
{
    menu.Use();
    return;
}

if (button.name.Equals("Drop", System.StringComparison.OrdinalIgnoreCase))
{
    menu.Drop();
    return;
}

if (button.name.Equals("Split", System.StringComparison.OrdinalIgnoreCase))
{
    menu.Split();
    return;
}

Debug.LogWarning($"Unknown context menu button: {button.name}");
                return;
            }
        }
    }
}

