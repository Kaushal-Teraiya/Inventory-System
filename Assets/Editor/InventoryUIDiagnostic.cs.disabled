using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public static class InventoryUIDiagnostic
{
    [MenuItem("Inventory System/Diagnose Current UI")]
    public static void Run()
    {
        var root = GameObject.Find("InventoryUI");
        var sb = new StringBuilder();

        if (root == null)
        {
            Debug.LogError("InventoryUI root not found.");
            return;
        }

        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            if (!t.name.StartsWith("Slot_") &&
                t.name != "SlotGrid" &&
                t.name != "Icon" &&
                t.name != "Selection")
                continue;

            var r = t.GetComponent<RectTransform>();
            var image = t.GetComponent<Image>();
            var button = t.GetComponent<Button>();

            sb.AppendLine($"OBJECT: {GetPath(t)}");
            if (r != null)
                sb.AppendLine($"  AnchorMin={r.anchorMin}, AnchorMax={r.anchorMax}, Pivot={r.pivot}, Position={r.anchoredPosition}, Size={r.sizeDelta}");
            if (image != null)
                sb.AppendLine($"  ImageColor={image.color}, RaycastTarget={image.raycastTarget}");
            if (button != null)
            {
                var c = button.colors;
                sb.AppendLine($"  ButtonTransition={button.transition}, TargetGraphic={(button.targetGraphic != null ? button.targetGraphic.name : "NULL")}");
                sb.AppendLine($"  Normal={c.normalColor}, Highlighted={c.highlightedColor}, Pressed={c.pressedColor}, Selected={c.selectedColor}");
            }
        }

        var path = Path.GetFullPath("InventoryUIDiagnostic.txt");
        File.WriteAllText(path, sb.ToString());
        Debug.Log($"Inventory UI diagnostic saved to: {path}");
    }

    private static string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
