using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InventorySystem.UI;

public static class InventoryUIGenerator
{
    [MenuItem("Inventory System/Generate Inventory UI")]
    public static void Generate()
    {
        var existing = GameObject.Find("InventoryUI");
        if (existing != null &&
            !EditorUtility.DisplayDialog(
                "Rebuild Inventory",
                "Replace the existing inventory UI?",
                "Rebuild", "Cancel"))
            return;

        if (existing != null)
            Object.DestroyImmediate(existing);

        var root = new GameObject("InventoryUI",
            typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster),
            typeof(InventoryUI));

        var canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        var panel = Make("InventoryPanel", root.transform,
            new Vector2(.5f,.5f), new Vector2(.5f,.5f),
            Vector2.zero, new Vector2(1160,760));
        panel.AddComponent<Image>().color = C("#11121B");

        var outline = panel.AddComponent<Outline>();
        outline.effectColor = C("#7656A8");
        outline.effectDistance = new Vector2(1,-1);

        MakeText("Title", panel.transform, "INVENTORY",
            new Vector2(0,1), new Vector2(1,1),
            new Vector2(0,-26), new Vector2(-64,54),
            30, FontStyles.Bold, TextAlignmentOptions.Center, C("#F2EDFF"));

        MakeText("CapacityLabel", panel.transform, "24 SLOTS",
            new Vector2(0,1), new Vector2(0,1),
            new Vector2(34,-82), new Vector2(500,28),
            14, FontStyles.Normal, TextAlignmentOptions.Left, C("#AAA3BD"));

        var grid = Make("SlotGrid", panel.transform,
            new Vector2(0,1), new Vector2(0,1),
            new Vector2(30,-120), new Vector2(510,510));
        var layout = grid.AddComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(88,88);
        layout.spacing = new Vector2(10,10);
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 5;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.padding = new RectOffset(2,2,2,2);

        for (int i = 0; i < 24; i++)
            MakeSlot(grid.transform, i);

        // Runtime InventoryUI owns the detail-panel contents.
        // Its layout is set to the right-hand column below.
        var details = Make("ItemDetailsPanel", panel.transform,
            new Vector2(1,1), new Vector2(1,1),
            new Vector2(-30,-120), new Vector2(560,510));
        details.AddComponent<Image>().color = C("#191925");

        var footer = Make("Footer", panel.transform,
            new Vector2(0,0), new Vector2(1,0),
            new Vector2(0,22), new Vector2(-60,48));

        MakeText("FooterHint", footer.transform,
            "Select an item  •  Drag to move  •  Right-click to split",
            new Vector2(0,0), new Vector2(1,1),
            Vector2.zero, Vector2.zero,
            14, FontStyles.Normal, TextAlignmentOptions.Left, C("#AAA3BD"));

        EnsureEventSystem();
        Selection.activeGameObject = root;
        Debug.Log("New inventory layout generated. Test in Play mode.");
    }

    private static GameObject Make(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var r = go.GetComponent<RectTransform>();
        r.anchorMin = anchorMin;
        r.anchorMax = anchorMax;
        r.pivot = new Vector2(
            (anchorMin.x + anchorMax.x) * .5f,
            (anchorMin.y + anchorMax.y) * .5f);
        r.anchoredPosition = position;
        r.sizeDelta = size;
        return go;
    }

    private static void MakeText(string name, Transform parent, string value,
        Vector2 amin, Vector2 amax, Vector2 position, Vector2 size,
        float fontSize, FontStyles style, TextAlignmentOptions alignment, Color color)
    {
        var go = Make(name, parent, amin, amax, position, size);
        var text = go.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = color;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        if (amin != amax)
        {
            var r = go.GetComponent<RectTransform>();
            r.offsetMin = new Vector2(24, 0);
            r.offsetMax = new Vector2(-24, 0);
        }
    }

    private static void MakeSlot(Transform parent, int index)
    {
        var go = Make("Slot_" + index, parent, Vector2.zero, Vector2.one,
            Vector2.zero, new Vector2(88,88));
        var image = go.AddComponent<Image>();
        image.color = C("#242431");
        image.raycastTarget = true;
        var button = go.AddComponent<Button>();
        button.targetGraphic = image;
        var colors = button.colors;
        colors.normalColor = C("#242431");
        colors.highlightedColor = C("#353249");
        colors.pressedColor = C("#51416C");
        colors.selectedColor = C("#51416C");
        button.colors = colors;

        go.AddComponent<InventorySlotUI>();

        var icon = Make("Icon", go.transform, new Vector2(.12f,.12f),
            new Vector2(.88f,.88f), Vector2.zero, Vector2.zero);
        var iconImage = icon.AddComponent<Image>();
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;
        iconImage.enabled = false;

        var selection = Make("Selection", go.transform, Vector2.zero,
            Vector2.one, Vector2.zero, Vector2.zero);
        var selectionImage = selection.AddComponent<Image>();
        selectionImage.color = new Color(.75f,.58f,1f,.14f);
        selectionImage.raycastTarget = false;
        selection.SetActive(false);

        var quantity = Make("Quantity", go.transform, Vector2.zero,
            Vector2.one, Vector2.zero, Vector2.zero);
        var qt = quantity.AddComponent<TextMeshProUGUI>();
        qt.fontSize = 16;
        qt.fontStyle = FontStyles.Bold;
        qt.alignment = TextAlignmentOptions.BottomRight;
        qt.color = Color.white;
        qt.raycastTarget = false;
        qt.margin = new Vector4(4,4,7,5);
    }

    private static Color C(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var color);
        return color;
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null)
            return;

        var go = new GameObject("EventSystem",
            typeof(UnityEngine.EventSystems.EventSystem),
            typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
        Undo.RegisterCreatedObjectUndo(go, "Create EventSystem");
    }
}
