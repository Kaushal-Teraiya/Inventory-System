using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using InventorySystem.UI;
using UnityEngine.InputSystem.UI;

public static class InventoryUIGenerator
{
    static Color C(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }

    [MenuItem("Inventory System/Generate Clean Inventory UI")]
    public static void Generate()
    {
        var old = GameObject.Find("InventoryUI");

        if (old != null && !EditorUtility.DisplayDialog(
            "Replace Inventory UI",
            "Replace the existing InventoryUI in the active scene?",
            "Replace", "Cancel"))
            return;

        if (old != null) Undo.DestroyObjectImmediate(old);

        var root = new GameObject("InventoryUI",
            typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster),
            typeof(InventoryUI));

        Undo.RegisterCreatedObjectUndo(root, "Generate Inventory UI");

        var canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        var panel = Rect("InventoryPanel", root.transform);
        Center(panel, Vector2.zero, new Vector2(1040, 600));
        panel.gameObject.AddComponent<Image>().color = C("#171321");
        Outline(panel.gameObject, C("#B45CFF"), 1.5f);

        Text(panel, "Title", "INVENTORY", 27, FontStyles.Bold,
            C("#F5EEFF"), new Vector2(28, -22),
            new Vector2(400, 38), new Vector2(0, 1));

        Text(panel, "Subtitle", "ITEMS  /  EQUIPMENT", 12,
            FontStyles.Bold, C("#AFA3C7"),
            new Vector2(30, -61), new Vector2(300, 22),
            new Vector2(0, 1));

        var accent = Rect("HeaderAccent", panel);
        Anchored(accent, new Vector2(30, -91),
            new Vector2(42, 3), new Vector2(0, 1));
        accent.gameObject.AddComponent<Image>().color = C("#B45CFF");

        var grid = Rect("SlotGrid", panel);
        Anchored(grid, new Vector2(26, -108),
            new Vector2(480, 420), new Vector2(0, 1));

        var layout = grid.gameObject.AddComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(82, 82);
        layout.spacing = new Vector2(10, 10);
        layout.padding = new RectOffset(3, 3, 3, 3);
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 5;
        layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        layout.startAxis = GridLayoutGroup.Axis.Horizontal;
        layout.childAlignment = TextAnchor.UpperLeft;

        for (int i = 0; i < 20; i++)
            MakeSlot(grid, i);

        var divider = Rect("DetailsDivider", panel);
        Anchored(divider, new Vector2(530, -108),
            new Vector2(1, 460), new Vector2(0, 1));
        divider.gameObject.AddComponent<Image>().color = C("#393047");

        // InventoryUI.BuildDetailsPanel creates the functional details card
        // at runtime. Keep this container inside the main panel.
        var details = Rect("ItemDetailsPanel", panel);
        Center(details, new Vector2(0, 0), new Vector2(280, 330));
        details.gameObject.AddComponent<Image>().color = C("#241934");
        Outline(details.gameObject, C("#8951C9"), 1f);

        Text(details, "Placeholder", "SELECT AN ITEM", 14,
            FontStyles.Bold, C("#B8ACCC"), Vector2.zero,
            new Vector2(240, 30), new Vector2(.5f, .5f));

        EnsureEventSystem();

        Selection.activeGameObject = root;
        Debug.Log("Inventory UI generated: 20 slots, 5 columns.");
    }

    static void MakeSlot(Transform parent, int index)
    {
        var slot = Rect("Slot_" + index, parent);
        Stretch(slot);

        var bg = slot.gameObject.AddComponent<Image>();
        bg.color = C("#30233F");
        bg.raycastTarget = true;

        var button = slot.gameObject.AddComponent<Button>();
        button.targetGraphic = bg;
        button.transition = Selectable.Transition.ColorTint;

        var colors = button.colors;
        colors.normalColor = C("#30233F");
        colors.highlightedColor = C("#49345E");
        colors.pressedColor = C("#65458B");
        colors.selectedColor = C("#65458B");
        colors.fadeDuration = .08f;
        button.colors = colors;

        slot.gameObject.AddComponent<InventorySlotUI>();

        var icon = Rect("Icon", slot);
        Stretch(icon);
        var ir = icon.GetComponent<RectTransform>();
        ir.anchorMin = new Vector2(.12f, .12f);
        ir.anchorMax = new Vector2(.88f, .88f);
        ir.offsetMin = Vector2.zero;
        ir.offsetMax = Vector2.zero;

        var image = icon.gameObject.AddComponent<Image>();
        image.preserveAspect = true;
        image.raycastTarget = false;
        image.enabled = false;

        var selection = Rect("Selection", slot);
        Stretch(selection);
        var si = selection.gameObject.AddComponent<Image>();
        si.color = new Color(1f, .72f, .25f, .13f);
        si.raycastTarget = false;
        selection.gameObject.SetActive(false);

        var quantity = Rect("Quantity", slot);
        Stretch(quantity);
        var q = quantity.gameObject.AddComponent<TextMeshProUGUI>();
        q.fontSize = 15;
        q.fontStyle = FontStyles.Bold;
        q.alignment = TextAlignmentOptions.BottomRight;
        q.color = Color.white;
        q.margin = new Vector4(3, 3, 6, 4);
        q.raycastTarget = false;

        Outline(slot.gameObject, C("#67468A"), 1f);
    }

    static RectTransform Rect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    static void Stretch(RectTransform r)
    {
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
        r.localScale = Vector3.one;
    }

    static void Center(RectTransform r, Vector2 pos, Vector2 size)
    {
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(.5f, .5f);
        r.anchoredPosition = pos;
        r.sizeDelta = size;
    }

    static void Anchored(RectTransform r, Vector2 pos, Vector2 size, Vector2 anchor)
    {
        r.anchorMin = r.anchorMax = anchor;
        r.pivot = anchor;
        r.anchoredPosition = pos;
        r.sizeDelta = size;
    }

    static void Text(Transform parent, string name, string value,
        float size, FontStyles style, Color color,
        Vector2 pos, Vector2 dimensions, Vector2 anchor)
    {
        var r = Rect(name, parent);
        Anchored(r, pos, dimensions, anchor);
        var t = r.gameObject.AddComponent<TextMeshProUGUI>();
        t.text = value;
        t.fontSize = size;
        t.fontStyle = style;
        t.color = color;
        t.raycastTarget = false;
        //t.enableWordWrapping = false;
        t.alignment = TextAlignmentOptions.Left;
    }

    static void Outline(GameObject go, Color color, float thickness)
    {
        var o = go.AddComponent<Outline>();
        o.effectColor = color;
        o.effectDistance = new Vector2(thickness, -thickness);
    }

    static void EnsureEventSystem()
    {
        var es = Object.FindFirstObjectByType<EventSystem>();

        if (es == null)
        {
            new GameObject("EventSystem",
                typeof(EventSystem), typeof(InputSystemUIInputModule));
        }
        else if (es.GetComponent<InputSystemUIInputModule>() == null)
        {
            var old = es.GetComponent<StandaloneInputModule>();
            if (old != null) Object.DestroyImmediate(old);
            es.gameObject.AddComponent<InputSystemUIInputModule>();
        }
    }
}
