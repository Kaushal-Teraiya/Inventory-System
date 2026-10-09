using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class InventoryUILayoutRepair
{
    [MenuItem("Inventory System/Apply Clean Inventory Layout")]
    public static void Apply()
    {
        var root = GameObject.Find("InventoryUI");
        if (root == null) { Debug.LogError("InventoryUI not found."); return; }

        var panelT = root.transform.Find("InventoryPanel");
        if (panelT == null) { Debug.LogError("InventoryPanel not found."); return; }

        var panel = panelT.GetComponent<RectTransform>();
        Undo.RecordObject(panel, "Apply Clean Inventory Layout");
        panel.anchorMin = panel.anchorMax = new Vector2(.5f, .5f);
        panel.pivot = new Vector2(.5f, .5f);
        panel.anchoredPosition = Vector2.zero;
        panel.sizeDelta = new Vector2(1120, 700);

        var bg = panel.GetComponent<Image>();
        if (bg != null) bg.color = new Color(.035f, .045f, .065f, .99f);

        // Remove obsolete decorative objects from previous polish passes.
        string[] obsolete = { "HeaderAccent", "HeaderAccentGlow", "InventoryFooter" };
        foreach (string name in obsolete)
        {
            var old = panel.Find(name);
            if (old != null) Undo.DestroyObjectImmediate(old.gameObject);
        }

        var titleT = panel.Find("Title");
        if (titleT != null)
        {
            var r = titleT.GetComponent<RectTransform>();
            r.anchorMin = r.anchorMax = new Vector2(.5f, 1);
            r.pivot = new Vector2(.5f, 1);
            r.anchoredPosition = new Vector2(0, -24);
            r.sizeDelta = new Vector2(1040, 54);

            var t = titleT.GetComponent<TMP_Text>();
            if (t != null)
            {
                t.text = "INVENTORY";
                t.fontSize = 30;
                t.fontStyle = FontStyles.Bold;
                t.characterSpacing = 2;
                t.alignment = TextAlignmentOptions.Center;
                t.color = new Color(.91f, .94f, 1f);
                t.raycastTarget = false;
            }
        }

        var gridT = panel.Find("SlotGrid");
        if (gridT != null)
        {
            var r = gridT.GetComponent<RectTransform>();
            r.anchorMin = r.anchorMax = new Vector2(0, 1);
            r.pivot = new Vector2(0, 1);
            r.anchoredPosition = new Vector2(30, -100);
            r.sizeDelta = new Vector2(510, 520);

            var grid = gridT.GetComponent<GridLayoutGroup>();
            if (grid != null)
            {
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = 5;
                grid.cellSize = new Vector2(88, 88);
                grid.spacing = new Vector2(10, 10);
                grid.padding = new RectOffset(4, 4, 4, 4);
                grid.childAlignment = TextAnchor.UpperLeft;
            }
        }

        // Find the runtime-created details panel and place it within the window.
        var details = panel.Find("ItemDetailsPanel");
        if (details != null)
        {
            var r = details.GetComponent<RectTransform>();
            r.anchorMin = r.anchorMax = new Vector2(1, 1);
            r.pivot = new Vector2(1, 1);
            r.anchoredPosition = new Vector2(-28, -100);
            r.sizeDelta = new Vector2(520, 520);

            var image = details.GetComponent<Image>();
            if (image != null) image.color = new Color(.06f, .075f, .105f, 1f);

            // Normalize existing non-button text to avoid overlapping labels.
            int textIndex = 0;
            foreach (var text in details.GetComponentsInChildren<TMP_Text>(true))
            {
                if (text.transform.parent.GetComponent<Button>() != null) continue;

                var tr = text.GetComponent<RectTransform>();
                if (tr == null) continue;

                tr.anchorMin = tr.anchorMax = new Vector2(0, 1);
                tr.pivot = new Vector2(0, 1);

                if (textIndex == 0)
                {
                    tr.anchoredPosition = new Vector2(24, -24);
                    tr.sizeDelta = new Vector2(460, 44);
                    text.fontSize = 22;
                    text.fontStyle = FontStyles.Bold;
                }
                else
                {
                    tr.anchoredPosition = new Vector2(24, -82);
                    tr.sizeDelta = new Vector2(460, 150);
                    text.fontSize = 16;
                    text.textWrappingMode = TextWrappingModes.Normal;
                }

                text.color = new Color(.86f, .9f, .97f);
                text.raycastTarget = false;
                textIndex++;
            }

            // Keep action buttons aligned in one column.
            float y = -290;
            foreach (var button in details.GetComponentsInChildren<Button>(true))
            {
                var br = button.GetComponent<RectTransform>();
                if (br == null) continue;
                br.anchorMin = br.anchorMax = new Vector2(.5f, 1);
                br.pivot = new Vector2(.5f, 1);
                br.anchoredPosition = new Vector2(0, y);
                br.sizeDelta = new Vector2(440, 42);
                y -= 52;

                var bi = button.GetComponent<Image>();
                if (bi != null) bi.color = new Color(.17f, .13f, .27f, 1f);

                var label = button.GetComponentInChildren<TMP_Text>(true);
                if (label != null)
                {
                    label.alignment = TextAlignmentOptions.Center;
                    label.fontSize = 15;
                    label.color = Color.white;
                    label.raycastTarget = false;
                }
            }
        }

        // Slot visuals and icon/quantity layout.
        foreach (var slot in panel.GetComponentsInChildren<InventorySystem.UI.InventorySlotUI>(true))
        {
            var sr = slot.GetComponent<RectTransform>();
            if (sr != null) sr.sizeDelta = new Vector2(88, 88);

            var si = slot.GetComponent<Image>();
            if (si != null) si.color = new Color(.075f, .09f, .125f, 1f);

            var iconT = slot.transform.Find("Icon");
            if (iconT != null)
            {
                var ir = iconT.GetComponent<RectTransform>();
                ir.anchorMin = new Vector2(.12f, .12f);
                ir.anchorMax = new Vector2(.88f, .88f);
                ir.offsetMin = ir.offsetMax = Vector2.zero;
                var ii = iconT.GetComponent<Image>();
                if (ii != null) ii.raycastTarget = false;
            }

            var quantityT = slot.transform.Find("Quantity");
            if (quantityT != null)
            {
                var qr = quantityT.GetComponent<RectTransform>();
                qr.anchorMin = Vector2.zero;
                qr.anchorMax = Vector2.one;
                qr.offsetMin = new Vector2(5, 4);
                qr.offsetMax = new Vector2(-6, -4);

                var qt = quantityT.GetComponent<TMP_Text>();
                if (qt != null)
                {
                    qt.alignment = TextAlignmentOptions.BottomRight;
                    qt.fontSize = 16;
                    qt.fontStyle = FontStyles.Bold;
                    qt.color = Color.white;
                    qt.raycastTarget = false;
                }
            }
        }

        EditorUtility.SetDirty(root);
        Debug.Log("Clean inventory layout applied. Test selection, drag/drop, split, and close in Play mode.");
    }
}
