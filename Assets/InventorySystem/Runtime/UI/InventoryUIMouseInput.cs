using System.Collections.Generic;
using InventorySystem.UI;
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

        private InventorySlotUI hoveredSlot;
        private InventorySlotUI draggedSlot;

        private void Awake()
        {
            if (raycaster == null)
                raycaster = GetComponent<GraphicRaycaster>();

            if (raycaster == null)
                raycaster = GetComponentInParent<GraphicRaycaster>();

            if (contextMenu == null)
                contextMenu = GetComponentInChildren<InventoryContextMenu>(true);

            var inventoryUI = GetComponent<InventoryUI>();

            if (contextMenu != null && inventoryUI != null)
                contextMenu.Initialize(inventoryUI);
        }

        private void Update()
        {
            if (raycaster == null || Mouse.current == null)
                return;

            // Context menu owns the mouse while it is open.
            if (contextMenu != null && contextMenu.gameObject.activeSelf)
            {
                if (Keyboard.current != null &&
                    Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    contextMenu.Close();
                    return;
                }

                if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    contextMenu.Close();
                    return;
                }

                return;
            }

            UpdateHoveredSlot();

            if (Mouse.current.leftButton.wasPressedThisFrame)
                StartDrag();

            if (Mouse.current.leftButton.wasReleasedThisFrame)
                EndDrag();

            if (Mouse.current.rightButton.wasPressedThisFrame)
                OpenContextMenu();
        }

        private void UpdateHoveredSlot()
        {
            var pointerData =
                new PointerEventData(EventSystem.current)
                {
                    position = Mouse.current.position.ReadValue()
                };

            var results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            InventorySlotUI slot = null;

            foreach (var result in results)
            {
                slot = result.gameObject
                    .GetComponentInParent<InventorySlotUI>();

                if (slot != null)
                    break;
            }

            if (slot == hoveredSlot)
                return;

            if (hoveredSlot != null && hoveredSlot != draggedSlot)
                hoveredSlot.SetSelected(false);

            hoveredSlot = slot;

            if (hoveredSlot != null && hoveredSlot != draggedSlot)
                hoveredSlot.SetSelected(true);
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
                var inventoryUI = GetComponent<InventoryUI>();

                var result = inventoryUI.Inventory.MoveItem(
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
            if (hoveredSlot == null ||
                hoveredSlot.BoundSlot == null ||
                hoveredSlot.BoundSlot.IsEmpty)
                return;

            if (contextMenu == null)
                return;

            contextMenu.Open(
                hoveredSlot.BoundSlot.Index);
        }
    }
}

