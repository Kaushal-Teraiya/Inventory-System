using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class InventoryUIBeautyPassEditor
{
    [MenuItem("Inventory System/Make Inventory Beautiful")]
    public static void Apply()
    {
        var root = GameObject.Find("InventoryUI");

        if (root == null)
        {
            Debug.LogError("InventoryUI not found.");
            return;
        }

        var panel = root.transform.Find("InventoryPanel");

        if (panel == null)
        {
            Debug.LogError("InventoryPanel not found.");
            return;
        }

        // Main panel
        var panelImage = panel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.color = new Color(0.025f, 0.032f, 0.05f, 0.985f);

        var outline = panel.GetComponent<Outline>();
        if (outline == null)
            outline = panel.gameObject.AddComponent<Outline>();

        outline.effectColor = new Color(0.22f, 0.45f, 0.72f, 0.45f);
        outline.effectDistance = new Vector2(2f, 2f);

        var shadow = panel.GetComponent<Shadow>();
        if (shadow == null)
            shadow = panel.gameObject.AddComponent<Shadow>();

        shadow.effectColor = new Color(0f, 0f, 0f, 0.8f);
        shadow.effectDistance = new Vector2(0f, -10f);

        // Header
        var title = panel.Find("Title");

        if (title != null)
        {
            var titleText = title.GetComponent<TMP_Text>();

            if (titleText != null)
            {
                titleText.text = "INVENTORY";
                titleText.fontSize = 34f;
                titleText.fontStyle = FontStyles.Bold;
                titleText.characterSpacing = 7f;
                titleText.color = new Color(0.88f, 0.94f, 1f, 1f);
                titleText.alignment = TextAlignmentOptions.Center;
            }
        }

        CreateAccent(panel, "HeaderAccent", 0f, -92f, 760f, 3f);
        CreateAccent(panel, "HeaderAccentGlow", 0f, -92f, 420f, 7f);

        // Slot grid
        var grid = panel.Find("SlotGrid");

        if (grid != null)
        {
            var layout = grid.GetComponent<GridLayoutGroup>();

            if (layout != null)
            {
                layout.cellSize = new Vector2(125f, 125f);
                layout.spacing = new Vector2(12f, 12f);
                layout.padding = new RectOffset(4, 4, 4, 4);
            }
        }

        // Slots
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            if (!t.name.StartsWith("Slot_"))
                continue;

            var image = t.GetComponent<Image>();

            if (image != null)
            {
                image.color =
                    new Color(0.055f, 0.07f, 0.095f, 1f);
            }

            var slotOutline = t.GetComponent<Outline>();

            if (slotOutline == null)
                slotOutline = t.gameObject.AddComponent<Outline>();

            slotOutline.effectColor =
                new Color(0.18f, 0.25f, 0.34f, 0.8f);

            slotOutline.effectDistance =
                new Vector2(2f, 2f);

            var slotShadow = t.GetComponent<Shadow>();

            if (slotShadow == null)
                slotShadow = t.gameObject.AddComponent<Shadow>();

            slotShadow.effectColor =
                new Color(0f, 0f, 0f, 0.7f);

            slotShadow.effectDistance =
                new Vector2(0f, -4f);

            var icon = t.Find("Icon");

            if (icon != null)
            {
                var iconRect = icon.GetComponent<RectTransform>();

                iconRect.anchorMin = new Vector2(0.13f, 0.13f);
                iconRect.anchorMax = new Vector2(0.87f, 0.87f);
                iconRect.offsetMin = Vector2.zero;
                iconRect.offsetMax = Vector2.zero;

                var iconImage = icon.GetComponent<Image>();

                if (iconImage != null)
                    iconImage.raycastTarget = false;
            }

            var quantity = t.Find("Quantity");

            if (quantity != null)
            {
                var quantityText = quantity.GetComponent<TMP_Text>();

                if (quantityText != null)
                {
                    quantityText.fontSize = 21f;
                    quantityText.fontStyle = FontStyles.Bold;
                    quantityText.color = Color.white;
                    quantityText.outlineWidth = 0.3f;
                    quantityText.outlineColor = Color.black;
                    quantityText.raycastTarget = false;
                }
            }
        }

        // Context menu
        var menu = root.transform.Find("ContextMenu");

        if (menu != null)
        {
            var menuImage = menu.GetComponent<Image>();

            if (menuImage != null)
                menuImage.color =
                    new Color(0.025f, 0.035f, 0.055f, 0.99f);

            var menuOutline = menu.GetComponent<Outline>();

            if (menuOutline == null)
                menuOutline = menu.gameObject.AddComponent<Outline>();

            menuOutline.effectColor =
                new Color(0.25f, 0.5f, 0.8f, 0.5f);

            menuOutline.effectDistance =
                new Vector2(2f, 2f);

            foreach (var button in menu.GetComponentsInChildren<Button>(true))
            {
                var buttonImage = button.GetComponent<Image>();

                if (buttonImage != null)
                {
                    buttonImage.color =
                        new Color(0.07f, 0.09f, 0.13f, 1f);
                    buttonImage.raycastTarget = true;
                }

                var text = button.GetComponentInChildren<TMP_Text>();

                if (text != null)
                {
                    text.fontSize = 18f;
                    text.fontStyle = FontStyles.Bold;
                    text.color =
                        new Color(0.88f, 0.93f, 1f, 1f);
                    text.alignment =
                        TextAlignmentOptions.Center;
                }
            }
        }

        // Footer label
        CreateFooter(panel);

        EditorUtility.SetDirty(root);

        Debug.Log("Inventory beauty pass applied.");
    }

    private static void CreateAccent(
        Transform parent,
        string name,
        float x,
        float y,
        float width,
        float height)
    {
        var existing = parent.Find(name);

        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(width, height);

        var image = go.GetComponent<Image>();

        image.color = name.Contains("Glow")
            ? new Color(0.2f, 0.55f, 1f, 0.12f)
            : new Color(0.25f, 0.65f, 1f, 0.8f);

        image.raycastTarget = false;
    }

    private static void CreateFooter(Transform panel)
    {
        var existing = panel.Find("InventoryFooter");

        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        var go = new GameObject(
            "InventoryFooter",
            typeof(RectTransform),
            typeof(TextMeshProUGUI));

        go.transform.SetParent(panel, false);

        var rect = go.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 18f);
        rect.sizeDelta = new Vector2(600f, 30f);

        var text = go.GetComponent<TextMeshProUGUI>();

        text.text = "24 SLOTS   •   DRAG TO MOVE   •   RIGHT CLICK FOR OPTIONS";
        text.fontSize = 12f;
        text.characterSpacing = 1.5f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.45f, 0.55f, 0.68f, 1f);
        text.raycastTarget = false;
    }
}
