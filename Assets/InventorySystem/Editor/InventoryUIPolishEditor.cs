using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class InventoryUIPolishEditor
{
    [MenuItem("Inventory System/Polish Inventory UI")]
    public static void Polish()
    {
        var root = GameObject.Find("InventoryUI");

        if (root == null)
        {
            Debug.LogError("InventoryUI not found.");
            return;
        }

        var canvas = root.GetComponent<Canvas>();

        if (canvas != null)
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        StylePanel(root);
        StyleTitle(root);
        StyleGrid(root);
        StyleSlots(root);
        StyleContextMenu(root);

        EditorUtility.SetDirty(root);

        Debug.Log("Inventory UI polished successfully.");
    }

    private static void StylePanel(GameObject root)
    {
        var panel = root.transform.Find("InventoryPanel");

        if (panel == null)
            return;

        var image = panel.GetComponent<Image>();

        if (image != null)
        {
            image.color = new Color(0.035f, 0.045f, 0.06f, 0.97f);
        }

        AddShadow(panel.gameObject, 0.45f, 8f, new Vector2(0f, -4f));
    }

    private static void StyleTitle(GameObject root)
    {
        var title = root.transform.Find("InventoryPanel/Title");

        if (title == null)
            return;

        var text = title.GetComponent<TMP_Text>();

        if (text == null)
            return;

        text.text = "INVENTORY";
        text.fontSize = 30f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.9f, 0.93f, 0.98f, 1f);
        text.characterSpacing = 4f;
    }

    private static void StyleGrid(GameObject root)
    {
        var grid = root.transform.Find("InventoryPanel/SlotGrid");

        if (grid == null)
            return;

        var layout = grid.GetComponent<GridLayoutGroup>();

        if (layout == null)
            return;

        layout.cellSize = new Vector2(82f, 82f);
        layout.spacing = new Vector2(8f, 8f);
        layout.padding = new RectOffset(8, 8, 8, 8);
        layout.childAlignment = TextAnchor.UpperCenter;
    }

    private static void StyleSlots(GameObject root)
    {
        var slots = root.GetComponentsInChildren<InventorySystem.UI.InventorySlotUI>(true);

        foreach (var slot in slots)
        {
            var go = slot.gameObject;
            var image = go.GetComponent<Image>();

            if (image != null)
            {
                image.color = new Color(
                    0.075f,
                    0.09f,
                    0.12f,
                    1f);
            }

            AddShadow(go, 0.3f, 3f, new Vector2(0f, -2f));

            var icon = go.transform.Find("Icon");

            if (icon != null)
            {
                var iconImage = icon.GetComponent<Image>();

                if (iconImage != null)
                    iconImage.raycastTarget = false;

                var rect = icon.GetComponent<RectTransform>();

                if (rect != null)
                {
                    rect.anchorMin = new Vector2(0.12f, 0.12f);
                    rect.anchorMax = new Vector2(0.88f, 0.88f);
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                }
            }

            var quantity = go.transform.Find("Quantity");

            if (quantity != null)
            {
                var text = quantity.GetComponent<TMP_Text>();

                if (text != null)
                {
                    text.fontSize = 18f;
                    text.fontStyle = FontStyles.Bold;
                    text.alignment = TextAlignmentOptions.BottomRight;
                    text.color = Color.white;
                    text.outlineWidth = 0.25f;
                    text.outlineColor = Color.black;
                }
            }
        }
    }

    private static void StyleContextMenu(GameObject root)
    {
        var menu = root.transform.Find("ContextMenu");

        if (menu == null)
            return;

        var image = menu.GetComponent<Image>();

        if (image != null)
        {
            image.color = new Color(
                0.045f,
                0.055f,
                0.075f,
                0.98f);
        }

        AddShadow(menu.gameObject, 0.55f, 10f, new Vector2(0f, -5f));

        foreach (var button in menu.GetComponentsInChildren<Button>(true))
        {
            var buttonImage = button.GetComponent<Image>();

            if (buttonImage != null)
            {
                buttonImage.color = new Color(
                    0.09f,
                    0.11f,
                    0.145f,
                    1f);
            }

            var text = button.GetComponentInChildren<TMP_Text>();

            if (text != null)
            {
                text.fontSize = 17f;
                text.fontStyle = FontStyles.Bold;
                text.color = new Color(
                    0.88f,
                    0.91f,
                    0.96f,
                    1f);
            }
        }
    }

    private static void AddShadow(
        GameObject target,
        float alpha,
        float distance,
        Vector2 effectDistance)
    {
        var shadow = target.GetComponent<Shadow>();

        if (shadow == null)
            shadow = target.AddComponent<Shadow>();

        shadow.effectColor = new Color(0f, 0f, 0f, alpha);
        shadow.effectDistance = effectDistance;
    }
}
