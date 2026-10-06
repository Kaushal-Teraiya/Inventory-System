using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class InventoryUIGenerator
{
    [MenuItem("Inventory System/Generate Inventory UI")]
    public static void Generate()
    {
        var existing = GameObject.Find("InventoryUI");

        if (existing != null)
        {
            if (!EditorUtility.DisplayDialog(
                "Inventory UI Exists",
                "An InventoryUI already exists. Delete and recreate it?",
                "Recreate",
                "Cancel"))
                return;

            Object.DestroyImmediate(existing);
        }

        var root = new GameObject(
            "InventoryUI",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        var canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        var panel = CreateUIObject(
            "InventoryPanel",
            root.transform);

        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.08f, 0.08f, 0.08f, 0.95f);

        SetSize(panel, 900, 650);
        Center(panel);

        var title = CreateUIObject(
            "Title",
            panel.transform);

        var titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "INVENTORY";
        titleText.fontSize = 32;
        titleText.alignment = TextAlignmentOptions.Center;

        SetAnchor(title, new Vector2(0.5f, 1f));
        SetPivot(title, new Vector2(0.5f, 1f));
        SetPosition(title, 0, -35);
        SetSize(title, 800, 60);

        var grid = CreateUIObject(
            "SlotGrid",
            panel.transform);

        var gridLayout = grid.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(100, 100);
        gridLayout.spacing = new Vector2(12, 12);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 6;
        gridLayout.padding = new RectOffset(20, 20, 20, 20);

        SetAnchor(grid, new Vector2(0.5f, 0.5f));
        SetPivot(grid, new Vector2(0.5f, 0.5f));
        SetPosition(grid, 0, -25);
        SetSize(grid, 700, 500);

        for (int i = 0; i < 24; i++)
            CreateSlot(grid.transform, i);

        Selection.activeGameObject = root;

        Debug.Log("Inventory UI generated successfully.");
    }

    private static GameObject CreateSlot(
        Transform parent,
        int index)
    {
        var slot = CreateUIObject(
            $"Slot_{index}",
            parent);

        var image = slot.AddComponent<Image>();
        image.color = new Color(0.16f, 0.16f, 0.16f, 1f);

        var button = slot.AddComponent<Button>();
        button.targetGraphic = image;

        var icon = CreateUIObject(
            "Icon",
            slot.transform);

        icon.AddComponent<Image>();

        SetAnchor(icon, new Vector2(0.5f, 0.5f));
        SetPivot(icon, new Vector2(0.5f, 0.5f));
        SetPosition(icon, 0, 5);
        SetSize(icon, 70, 70);

        var quantity = CreateUIObject(
            "Quantity",
            slot.transform);

        var quantityText = quantity.AddComponent<TextMeshProUGUI>();
        quantityText.text = "";
        quantityText.fontSize = 20;
        quantityText.alignment = TextAlignmentOptions.BottomRight;

        SetAnchor(quantity, new Vector2(1, 0));
        SetPivot(quantity, new Vector2(1, 0));
        SetPosition(quantity, -5, 5);
        SetSize(quantity, 40, 30);

        return slot;
    }

    private static GameObject CreateUIObject(
        string name,
        Transform parent)
    {
        var obj = new GameObject(
            name,
            typeof(RectTransform));

        obj.transform.SetParent(parent, false);
        return obj;
    }

    private static void SetSize(
        GameObject obj,
        float width,
        float height)
    {
        var rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, height);
    }

    private static void SetPosition(
        GameObject obj,
        float x,
        float y)
    {
        var rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(x, y);
    }

    private static void Center(GameObject obj)
    {
        SetAnchor(obj, new Vector2(0.5f, 0.5f));
        SetPivot(obj, new Vector2(0.5f, 0.5f));
        SetPosition(obj, 0, 0);
    }

    private static void SetAnchor(
        GameObject obj,
        Vector2 anchor)
    {
        var rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
    }

    private static void SetPivot(
        GameObject obj,
        Vector2 pivot)
    {
        obj.GetComponent<RectTransform>().pivot = pivot;
    }
}
