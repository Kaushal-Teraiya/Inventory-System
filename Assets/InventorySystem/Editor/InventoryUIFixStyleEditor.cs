using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class InventoryUIFixStyleEditor
{
    [MenuItem("Inventory System/Fix Inventory Visuals")]
    public static void Fix()
    {
        var root = GameObject.Find("InventoryUI");

        if (root == null)
        {
            Debug.LogError("InventoryUI not found.");
            return;
        }

        var panel = root.transform.Find("InventoryPanel");

        if (panel != null)
        {
            var rect = panel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700f, 620f);

            var image = panel.GetComponent<Image>();
            if (image != null)
                image.color = new Color(0.035f, 0.045f, 0.06f, 0.98f);
        }

        var title = root.transform.Find("InventoryPanel/Title");

        if (title != null)
        {
            var rect = title.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600f, 55f);
            rect.anchoredPosition = new Vector2(0f, -35f);

            var text = title.GetComponent<TMP_Text>();

            if (text != null)
            {
                text.text = "INVENTORY";
                text.fontSize = 32f;
                text.fontStyle = FontStyles.Bold;
                text.alignment = TextAlignmentOptions.Center;
                text.color = new Color(0.92f, 0.94f, 0.98f, 1f);
                text.characterSpacing = 5f;
            }
        }

        var grid = root.transform.Find("InventoryPanel/SlotGrid");

        if (grid != null)
        {
            var rect = grid.GetComponent<RectTransform>();

            rect.sizeDelta = new Vector2(610f, 500f);
            rect.anchoredPosition = new Vector2(0f, -65f);

            var layout = grid.GetComponent<GridLayoutGroup>();

            if (layout != null)
            {
                layout.cellSize = new Vector2(90f, 90f);
                layout.spacing = new Vector2(10f, 10f);
                layout.padding = new RectOffset(5, 5, 5, 5);
                layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                layout.constraintCount = 6;
            }
        }

        // Style the actual slot GameObjects by name.
        foreach (var transform in root.GetComponentsInChildren<Transform>(true))
        {
            if (!transform.name.StartsWith("Slot_"))
                continue;

            var slotImage = transform.GetComponent<Image>();

            if (slotImage != null)
            {
                slotImage.color =
                    new Color(0.075f, 0.09f, 0.12f, 1f);

                slotImage.raycastTarget = true;
            }

            var shadow = transform.GetComponent<Shadow>();

            if (shadow == null)
                shadow = transform.gameObject.AddComponent<Shadow>();

            shadow.effectColor =
                new Color(0f, 0f, 0f, 0.55f);

            shadow.effectDistance =
                new Vector2(0f, -3f);

            var icon = transform.Find("Icon");

            if (icon != null)
            {
                var iconRect = icon.GetComponent<RectTransform>();

                iconRect.anchorMin = new Vector2(0.10f, 0.10f);
                iconRect.anchorMax = new Vector2(0.90f, 0.90f);
                iconRect.offsetMin = Vector2.zero;
                iconRect.offsetMax = Vector2.zero;

                var iconImage = icon.GetComponent<Image>();

                if (iconImage != null)
                    iconImage.raycastTarget = false;
            }

            var quantity = transform.Find("Quantity");

            if (quantity != null)
            {
                var text = quantity.GetComponent<TMP_Text>();

                if (text != null)
                {
                    text.fontSize = 18f;
                    text.fontStyle = FontStyles.Bold;
                    text.color = Color.white;
                    text.alignment =
                        TextAlignmentOptions.BottomRight;
                    text.raycastTarget = false;
                }
            }
        }

        // Context menu
        var menu = root.transform.Find("ContextMenu");

        if (menu != null)
        {
            var menuRect = menu.GetComponent<RectTransform>();
            menuRect.sizeDelta = new Vector2(210f, 175f);

            var menuImage = menu.GetComponent<Image>();

            if (menuImage != null)
            {
                menuImage.color =
                    new Color(0.045f, 0.055f, 0.075f, 0.98f);
            }

            foreach (var button in menu.GetComponentsInChildren<Button>(true))
            {
                var buttonRect = button.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(185f, 42f);

                var buttonImage = button.GetComponent<Image>();

                if (buttonImage != null)
                {
                    buttonImage.color =
                        new Color(0.10f, 0.12f, 0.16f, 1f);
                    buttonImage.raycastTarget = true;
                }

                var text = button.GetComponentInChildren<TMP_Text>();

                if (text != null)
                {
                    text.fontSize = 18f;
                    text.fontStyle = FontStyles.Bold;
                    text.alignment = TextAlignmentOptions.Center;
                    text.color =
                        new Color(0.9f, 0.92f, 0.96f, 1f);
                }
            }
        }

        EditorUtility.SetDirty(root);

        Debug.Log("Inventory UI visual layout fixed.");
    }
}
