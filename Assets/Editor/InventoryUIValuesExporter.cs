using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text;
using System.Linq;

public static class InventoryUIValuesExporter
{
    [MenuItem("Inventory System/Export Current UI Values")]
    public static void Export()
    {
        var root = GameObject.Find("InventoryUI");
        if (root == null)
        {
            Debug.LogError("InventoryUI not found in the open scene.");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("INVENTORY UI SCENE VALUES");
        sb.AppendLine("Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().path);
        sb.AppendLine("Game view reference: inspect CanvasScaler below.");
        sb.AppendLine();

        foreach (var canvas in root.GetComponentsInChildren<Canvas>(true))
        {
            var scaler = canvas.GetComponent<CanvasScaler>();
            sb.AppendLine("[CANVAS] " + PathOf(canvas.transform));
            sb.AppendLine("RenderMode=" + canvas.renderMode);
            if (scaler != null)
            {
                sb.AppendLine("ScaleMode=" + scaler.uiScaleMode);
                sb.AppendLine("ReferenceResolution=" + scaler.referenceResolution);
                sb.AppendLine("MatchMode=" + scaler.screenMatchMode);
                sb.AppendLine("MatchWidthOrHeight=" + scaler.matchWidthOrHeight);
            }
            sb.AppendLine();
        }

        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            var go = t.gameObject;
            var r = go.GetComponent<RectTransform>();
            sb.AppendLine("OBJECT: " + PathOf(t));
            sb.AppendLine("Active=" + go.activeSelf);

            if (r != null)
            {
                sb.AppendLine("  AnchorMin=" + r.anchorMin);
                sb.AppendLine("  AnchorMax=" + r.anchorMax);
                sb.AppendLine("  Pivot=" + r.pivot);
                sb.AppendLine("  AnchoredPosition=" + r.anchoredPosition);
                sb.AppendLine("  SizeDelta=" + r.sizeDelta);
                sb.AppendLine("  OffsetMin=" + r.offsetMin);
                sb.AppendLine("  OffsetMax=" + r.offsetMax);
                sb.AppendLine("  Rect=" + r.rect);
                sb.AppendLine("  WorldCorners=" + Corners(r));
            }

            var image = go.GetComponent<Image>();
            if (image != null)
                sb.AppendLine("  Image: Color=" + image.color +
                    ", Sprite=" + (image.sprite != null ? image.sprite.name : "null") +
                    ", Enabled=" + image.enabled +
                    ", Raycast=" + image.raycastTarget);

            var text = go.GetComponent<TMP_Text>();
            if (text != null)
                sb.AppendLine("  TMP: Text=[" + text.text.Replace("\n", "\\n") +
                    "], FontSize=" + text.fontSize +
                    ", Alignment=" + text.alignment +
                    ", Color=" + text.color +
                    ", Wrapping=" + text.textWrappingMode);

            var grid = go.GetComponent<GridLayoutGroup>();
            if (grid != null)
                sb.AppendLine("  Grid: Cell=" + grid.cellSize +
                    ", Spacing=" + grid.spacing +
                    ", Padding=" + grid.padding.left + "," + grid.padding.right +
                    "," + grid.padding.top + "," + grid.padding.bottom +
                    ", Constraint=" + grid.constraint +
                    ", Columns=" + grid.constraintCount +
                    ", ChildAlignment=" + grid.childAlignment);

            var group = go.GetComponent<HorizontalOrVerticalLayoutGroup>();
            if (group != null)
                sb.AppendLine("  LayoutGroup: Spacing=" + group.spacing +
                    ", Padding=" + group.padding.left + "," + group.padding.right +
                    "," + group.padding.top + "," + group.padding.bottom +
                    ", ChildAlignment=" + group.childAlignment +
                    ", ControlWidth=" + group.childControlWidth +
                    ", ControlHeight=" + group.childControlHeight +
                    ", ExpandWidth=" + group.childForceExpandWidth +
                    ", ExpandHeight=" + group.childForceExpandHeight);

            var element = go.GetComponent<LayoutElement>();
            if (element != null)
                sb.AppendLine("  LayoutElement: Min=" + element.minWidth + "x" + element.minHeight +
                    ", Preferred=" + element.preferredWidth + "x" + element.preferredHeight +
                    ", Flexible=" + element.flexibleWidth + "x" + element.flexibleHeight);

            var button = go.GetComponent<Button>();
            if (button != null)
                sb.AppendLine("  Button: Interactable=" + button.interactable +
                    ", Transition=" + button.transition);

            var slot = go.GetComponent<InventorySystem.UI.InventorySlotUI>();
            if (slot != null)
                sb.AppendLine("  InventorySlotUI: SlotIndex=" + slot.SlotIndex);

            sb.AppendLine();
        }

        var path = Path.Combine(Application.dataPath, "../InventoryUI_SceneValues.txt");
        File.WriteAllText(path, sb.ToString());
        Debug.Log("Exported inventory UI values to: " + path);
        EditorUtility.RevealInFinder(path);
    }

    private static string PathOf(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    private static string Corners(RectTransform r)
    {
        var c = new Vector3[4];
        r.GetWorldCorners(c);
        return string.Join(" | ", c.Select(v => v.ToString("F1")));
    }
}
