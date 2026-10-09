using InventorySystem.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public sealed class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image selection;
        [SerializeField] private TMP_Text quantityText;

        private InventorySlot boundSlot;

        public InventorySlot BoundSlot => boundSlot;

        private void Awake()
        {
            if (icon == null)
                icon = transform.Find("Icon")?.GetComponent<Image>();

            if (quantityText == null)
                quantityText = transform.Find("Quantity")?.GetComponent<TMP_Text>();

            if (selection == null)
                selection = transform.Find("Selection")?.GetComponent<Image>();

            if (selection == null)
                selection = GetComponent<Image>();

            if (icon != null)
                icon.raycastTarget = false;

            if (quantityText != null)
                quantityText.raycastTarget = false;

            var button = GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(Select);

            SetSelected(false);

            if (icon == null)
                Debug.LogError($"[{name}] Missing Image child named 'Icon'.", this);

            if (quantityText == null)
                Debug.LogError($"[{name}] Missing TMP_Text child named 'Quantity'.", this);
        }

        public void Bind(InventorySlot slot)
        {
            boundSlot = slot;
            Refresh();
        }

        public void Refresh()
        {
            if (boundSlot == null)
                return;

            bool hasItem = !boundSlot.IsEmpty;
            var definition = hasItem ? boundSlot.Stack.Item?.Definition : null;
            var sprite = definition != null ? definition.Icon : null;

            if (icon != null)
            {
                icon.sprite = sprite;
                icon.enabled = hasItem && sprite != null;
            }

            if (quantityText != null)
            {
                quantityText.text = hasItem && boundSlot.Stack.Quantity > 1
                    ? boundSlot.Stack.Quantity.ToString()
                    : string.Empty;
            }
        }
        public void Select()
        {
            if (boundSlot == null)
                return;

            var inventoryUI = GetComponentInParent<InventoryUI>();
            if (inventoryUI != null)
                inventoryUI.SelectSlot(boundSlot.Index);
        }

        public void SetSelected(bool selected)
        {
            if (selection == null)
                return;

            selection.color = selected
                ? new Color(1f, 0.85f, 0.2f, 1f)
                : new Color(0.16f, 0.16f, 0.16f, 1f);
        }
    }
}
