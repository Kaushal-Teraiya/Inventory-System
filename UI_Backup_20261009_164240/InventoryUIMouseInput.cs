using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public sealed class InventoryUIMouseInput : MonoBehaviour
    {
        private InventoryUI ui;
        private GraphicRaycaster raycaster;
        private InventorySlotUI dragSource;

        private void Awake()
        {
            ui = GetComponent<InventoryUI>();
            raycaster = GetComponent<GraphicRaycaster>();
            if (raycaster == null) raycaster = GetComponentInParent<GraphicRaycaster>();
        }

        private void Update()
        {
            if (ui == null || raycaster == null || Mouse.current == null || EventSystem.current == null) return;
            if (Mouse.current.leftButton.wasPressedThisFrame) dragSource = GetSlotUnderMouse();
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                var target = GetSlotUnderMouse();
                if (dragSource != null && target != null && dragSource != target)
                    ui.MoveSlots(dragSource.BoundSlot.Index, target.BoundSlot.Index);
                else if (target != null)
                    ui.SelectSlot(target.BoundSlot.Index);
                dragSource = null;
            }
        }

        private InventorySlotUI GetSlotUnderMouse()
        {
            var pointer = new PointerEventData(EventSystem.current)
            { position = Mouse.current.position.ReadValue() };
            var hits = new List<RaycastResult>();
            raycaster.Raycast(pointer, hits);
            foreach (var hit in hits)
            {
                var slot = hit.gameObject.GetComponentInParent<InventorySlotUI>();
                if (slot != null && slot.BoundSlot != null) return slot;
            }
            return null;
        }
    }
}
