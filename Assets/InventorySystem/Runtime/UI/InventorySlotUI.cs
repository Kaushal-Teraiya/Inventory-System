
using System;
using InventorySystem.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public sealed class InventorySlotUI : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        public event Action<int> SlotSelected;
        public event Action<int, int> SplitRequested;
        public event Action<int, int> SlotMoveRequested;

        [Header("References")]
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private Image selection;
        [SerializeField] private Image lockIcon;
        [SerializeField] private Sprite lockIconSprite;
        [SerializeField] private TMPro.TMP_Text quantityText;

        [Header("Colors")]
        [SerializeField] private Color normalColor = new(0.141f, 0.086f, 0.259f);
        [SerializeField] private Color hoverColor = new(0.22f, 0.125f, 0.373f);
        [SerializeField] private Color pressedColor = new(0.086f, 0.051f, 0.161f);
        [SerializeField] private Color selectedColor = new(0.286f, 0.188f, 0.114f);
        [SerializeField] private Color normalBorder = new(0.502f, 0.329f, 0.780f);
        [SerializeField] private Color hoverBorder = new(0.333f, 0.965f, 0.910f);
        [SerializeField] private Color pressedBorder = new(0.788f, 0.655f, 1f);
        [SerializeField] private Color selectedBorder = new(1f, 0.820f, 0.400f);

        private InventorySlot boundSlot;
        private Outline outline;
        private bool isSelected;
        private bool isHovered;
        private bool isPressed;
        private CanvasGroup previewCanvasGroup;
        private GameObject dragPreview;

        public InventorySlot BoundSlot => boundSlot;
        public int SlotIndex => boundSlot?.Index ?? -1;

        private void Awake()
        {
            if (background == null)
                background = GetComponent<Image>();

            if (icon == null)
                icon = transform.Find("SlotContent/ItemIcon")?.GetComponent<Image>()
                    ?? transform.Find("Icon")?.GetComponent<Image>();

            if (quantityText == null)
                quantityText = transform.Find("SlotContent/QuantityLabel")?.GetComponent<TMPro.TMP_Text>()
                    ?? transform.Find("Quantity")?.GetComponent<TMPro.TMP_Text>();

            if (selection == null)
                selection = transform.Find("Selection")?.GetComponent<Image>();

            if (lockIcon == null)
                lockIcon = transform.Find("LockIcon")?.GetComponent<Image>();

            if (background == null)
                background = GetComponentInChildren<Image>();

            if (background != null)
            {
                outline = background.GetComponent<Outline>();
                if (outline == null)
                    outline = background.gameObject.AddComponent<Outline>();

                outline.effectDistance = new Vector2(2f, -2f);
                outline.useGraphicAlpha = true;
            }

            if (icon != null)
                icon.raycastTarget = false;

            if (quantityText != null)
                quantityText.raycastTarget = false;

            if (lockIcon != null)
            {
                lockIcon.sprite = lockIconSprite != null ? lockIconSprite : lockIcon.sprite;
                lockIcon.raycastTarget = false;
                lockIcon.preserveAspect = true;
            }

            SetSelected(false);
            ApplyStyle();
        }

        public void Bind(InventorySlot slot)
        {
            boundSlot = slot;
            Refresh();
        }

        public void Refresh()
        {
            if (boundSlot == null)
            {
                if (icon != null) icon.enabled = false;
                if (quantityText != null) quantityText.text = string.Empty;
                if (lockIcon != null) lockIcon.enabled = false;
                return;
            }

            //bool locked = boundSlot.IsLocked;
            bool hasItem = !boundSlot.IsEmpty;
            var definition = hasItem ? boundSlot.Stack.Item?.Definition : null;
            Sprite itemSprite = definition != null ? definition.Icon : null;

            if (lockIcon != null)
            {
                lockIcon.enabled = false;
                lockIcon.gameObject.SetActive(false);
            }

            if (icon != null)
            {
                icon.sprite = itemSprite;
                icon.enabled = hasItem && itemSprite != null;
                icon.preserveAspect = true;
            }

            if (quantityText != null)
            {
                quantityText.text = hasItem && boundSlot.Stack.Quantity > 1
                    ? boundSlot.Stack.Quantity.ToString()
                    : string.Empty;
            }
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;

            if (selection != null)
            {
                selection.enabled = selected;
                selection.gameObject.SetActive(selected);
            }

            ApplyStyle();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
            ApplyStyle();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            isPressed = false;
            ApplyStyle();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isPressed = true;
            ApplyStyle();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            ApplyStyle();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (SlotIndex < 0)
                return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                SlotSelected?.Invoke(SlotIndex);

                var inventoryUI = GetComponentInParent<InventoryUI>();
                inventoryUI?.SelectSlot(SlotIndex);
            }
            else if (eventData.button == PointerEventData.InputButton.Right &&
                     boundSlot != null && !boundSlot.IsEmpty)
            {
                int splitQuantity = boundSlot.Stack.Quantity / 2;

                if (splitQuantity > 0)
                {
                    SplitRequested?.Invoke(SlotIndex, splitQuantity);
                    eventData.Use();
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (boundSlot == null || boundSlot.IsEmpty || icon == null ||
                icon.sprite == null || eventData.button != PointerEventData.InputButton.Left)
                return;

            dragPreview = new GameObject("InventoryDragPreview",
                typeof(RectTransform), typeof(CanvasGroup), typeof(Image));

            var previewRect = dragPreview.GetComponent<RectTransform>();
            previewRect.sizeDelta = new Vector2(48f, 48f);

            var previewImage = dragPreview.GetComponent<Image>();
            previewImage.sprite = icon.sprite;
            previewImage.preserveAspect = true;
            previewImage.raycastTarget = false;

            previewCanvasGroup = dragPreview.GetComponent<CanvasGroup>();
            previewCanvasGroup.blocksRaycasts = false;
            previewCanvasGroup.interactable = false;

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                dragPreview.transform.SetParent(canvas.rootCanvas.transform, false);
                previewRect.position = eventData.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragPreview != null)
                dragPreview.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (dragPreview != null)
                Destroy(dragPreview);

            dragPreview = null;
            previewCanvasGroup = null;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var sourceUI = eventData.pointerDrag?.GetComponent<InventorySlotUI>();

            if (sourceUI == null || sourceUI == this ||
                sourceUI.SlotIndex < 0 || SlotIndex < 0)
                return;

            SlotMoveRequested?.Invoke(sourceUI.SlotIndex, SlotIndex);

            var inventoryUI = GetComponentInParent<InventoryUI>();
            inventoryUI?.MoveSlots(sourceUI.SlotIndex, SlotIndex);
        }

        private void ApplyStyle()
        {
            if (background != null)
                background.color = isSelected ? selectedColor :
                    isPressed ? pressedColor :
                    isHovered ? hoverColor : normalColor;

            if (outline != null)
                outline.effectColor = isSelected ? selectedBorder :
                    isPressed ? pressedBorder :
                    isHovered ? hoverBorder : normalBorder;
        }
    }
}
