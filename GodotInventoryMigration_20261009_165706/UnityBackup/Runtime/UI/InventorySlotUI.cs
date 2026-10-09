using UnityEngine;
using UnityEngine.UI;
using InventorySystem.Inventory;
using UnityEngine.EventSystems;
using TMPro;

namespace InventorySystem.UI
{
    public sealed class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image selection;
        [SerializeField] private TMP_Text quantityText;

        private InventorySlot boundSlot;
        private Image background;
        private Button button;
        private bool isHovered;
        private bool isSelected;

        private static readonly Color NormalColor = new Color(0.141f, 0.141f, 0.192f, 1f);
        private static readonly Color HoverColor = new Color(0.208f, 0.196f, 0.286f, 1f);
        private static readonly Color SelectedColor = new Color(0.318f, 0.255f, 0.424f, 1f);

        public InventorySlot BoundSlot => boundSlot;

        private void Awake()
        {
            background = GetComponent<Image>();
            button = GetComponent<Button>();

            if (icon == null)
                icon = transform.Find("Icon")?.GetComponent<Image>();

            if (selection == null)
                selection = transform.Find("Selection")?.GetComponent<Image>();

            if (quantityText == null)
                quantityText = transform.Find("Quantity")?.GetComponent<TMP_Text>();

            if (button != null)
            {
                button.transition = Selectable.Transition.None;
                button.onClick.AddListener(Select);
            }

            if (icon != null)
            {
                if (icon.transform.parent != transform)
                    icon.transform.SetParent(transform, false);

                var rect = icon.rectTransform;
                rect.anchorMin = new Vector2(0.12f, 0.12f);
                rect.anchorMax = new Vector2(0.88f, 0.88f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
                icon.raycastTarget = false;
                icon.preserveAspect = true;
            }

            if (quantityText != null)
                quantityText.raycastTarget = false;

            if (selection != null)
            {
                selection.raycastTarget = false;
                selection.color = new Color(0.75f, 0.58f, 1f, 0.14f);
                selection.gameObject.SetActive(false);
            }

            if (background == null)
                Debug.LogError($"[{name}] Missing slot background Image.", this);

            if (icon == null)
                Debug.LogError($"[{name}] Missing Icon child with Image.", this);

            if (quantityText == null)
                Debug.LogError($"[{name}] Missing Quantity child with TMP_Text.", this);

            UpdateVisual();
        }

        public void Bind(InventorySlot slot)
        {
            boundSlot = slot;
            Refresh();
        }

        public void Refresh()
        {
            if (boundSlot == null) return;

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
                quantityText.text =
                    hasItem && boundSlot.Stack.Quantity > 1
                    ? boundSlot.Stack.Quantity.ToString()
                    : string.Empty;
            }
        }

        public void Select()
        {
            if (boundSlot == null) return;

            var inventoryUI = GetComponentInParent<InventoryUI>();
            if (inventoryUI != null)
                inventoryUI.SelectSlot(boundSlot.Index);
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;

            if (selection != null)
                selection.gameObject.SetActive(selected);

            UpdateVisual();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
            UpdateVisual();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (background == null) return;

            background.color = isSelected
                ? SelectedColor
                : isHovered ? HoverColor : NormalColor;

            if (selection != null)
                selection.gameObject.SetActive(isSelected);
        }
    }
}

