using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class InventoryUIContainmentFixEditor
{
    [MenuItem("Inventory System/Fix Inventory Containment")]
    public static void Fix()
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

        // 6 columns × 125 + 5 gaps × 12 = 810
        // 4 rows × 125 + 3 gaps × 12 = 536
        // Add generous padding for header/footer.
        var panelRect = panel.GetComponent<RectTransform>();

        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(940f, 760f);

        var grid = panel.Find("SlotGrid");

        if (grid != null)
        {
            var gridRect = grid.GetComponent<RectTransform>();

            gridRect.anchorMin = new Vector2(0.5f, 0.5f);
            gridRect.anchorMax = new Vector2(0.5f, 0.5f);
            gridRect.pivot = new Vector2(0.5f, 0.5f);

            // Keep the grid completely inside the panel.
            gridRect.anchoredPosition = new Vector2(0f, -35f);
            gridRect.sizeDelta = new Vector2(840f, 555f);

            var layout = grid.GetComponent<GridLayoutGroup>();

            if (layout != null)
            {
                layout.cellSize = new Vector2(125f, 125f);
                layout.spacing = new Vector2(12f, 12f);
                layout.padding = new RectOffset(5, 5, 5, 5);
                layout.constraint =
                    GridLayoutGroup.Constraint.FixedColumnCount;
                layout.constraintCount = 6;
            }
        }

        // Keep title comfortably inside the panel.
        var title = panel.Find("Title");

        if (title != null)
        {
            var titleRect = title.GetComponent<RectTransform>();

            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);

            titleRect.anchoredPosition =
                new Vector2(0f, -25f);
        }

        // Keep footer inside the panel.
        var footer = panel.Find("InventoryFooter");

        if (footer != null)
        {
            var footerRect = footer.GetComponent<RectTransform>();

            footerRect.anchorMin =
                new Vector2(0.5f, 0f);

            footerRect.anchorMax =
                new Vector2(0.5f, 0f);

            footerRect.pivot =
                new Vector2(0.5f, 0f);

            footerRect.anchoredPosition =
                new Vector2(0f, 12f);
        }

        EditorUtility.SetDirty(root);

        Debug.Log(
            "Inventory panel now properly contains the entire slot grid.");
    }
}
