using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public sealed class InventoryUIMouseInput : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float dragThreshold = 8f;

        private InventoryUI ui;
        private GraphicRaycaster raycaster;
        private InventorySlotUI dragSource;
        private Vector2 pressPosition;
        private bool isDragging;

        private void Awake()
        {
            ui = GetComponent<InventoryUI>();

            raycaster = GetComponent<GraphicRaycaster>();
            if (raycaster == null)
                raycaster = GetComponentInParent<GraphicRaycaster>();
        }

        private void Update()
        {
            if (ui == null || raycaster == null ||
                Mouse.current == null || EventSystem.current == null)
                return;

            var mouse = Mouse.current;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                dragSource = GetSlotUnderMouse();
                pressPosition = mouse.position.ReadValue();
                isDragging = false;
            }

            if (dragSource != null && mouse.leftButton.isPressed)
            {
                Vector2 currentPosition = mouse.position.ReadValue();

                if (!isDragging &&
                    Vector2.Distance(pressPosition, currentPosition) >= dragThreshold)
                {
                    isDragging = true;
                }
            }

            if (mouse.leftButton.wasReleasedThisFrame)
            {
                var target = GetSlotUnderMouse();

                if (isDragging &&
                    dragSource != null &&
                    target != null &&
                    target != dragSource &&
                    dragSource.BoundSlot != null &&
                    target.BoundSlot != null)
                {
                    ui.MoveSlots(
                        dragSource.BoundSlot.Index,
                        target.BoundSlot.Index
                    );
                }
                else if (!isDragging && target != null && target.BoundSlot != null)
                {
                    ui.SelectSlot(target.BoundSlot.Index);
                }

                dragSource = null;
                isDragging = false;
            }
        }

        private InventorySlotUI GetSlotUnderMouse()
        {
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };

            var hits = new List<RaycastResult>();
            raycaster.Raycast(pointer, hits);

            foreach (var hit in hits)
            {
                var slot = hit.gameObject.GetComponentInParent<InventorySlotUI>();

                if (slot != null && slot.BoundSlot != null)
                    return slot;
            }

            return null;
        }
    }
}
