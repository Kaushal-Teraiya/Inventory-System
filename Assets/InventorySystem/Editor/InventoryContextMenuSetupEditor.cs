using InventorySystem.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class InventoryContextMenuSetupEditor
{
    [MenuItem("Inventory System/Create Context Menu")]
    public static void Create()
    {
        var root = GameObject.Find("InventoryUI");

        if (root == null)
        {
            Debug.LogError("InventoryUI not found.");
            return;
        }

        var menu = new GameObject(
            "ContextMenu",
            typeof(RectTransform),
            typeof(Image),
            typeof(InventoryContextMenu));

        menu.transform.SetParent(root.transform, false);

        var rect = menu.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(180, 150);

        var image = menu.GetComponent<Image>();
        image.color = new Color(0.08f, 0.08f, 0.08f, 0.95f);

        CreateButton(menu.transform, "Use", 35);
        CreateButton(menu.transform, "Drop", 0);
        CreateButton(menu.transform, "Split", -35);

        var context = menu.GetComponent<InventoryContextMenu>();

        var buttons = menu.GetComponentsInChildren<Button>();

        SerializedObject serialized = new SerializedObject(context);
        serialized.FindProperty("useButton").objectReferenceValue = buttons[0];
        serialized.FindProperty("dropButton").objectReferenceValue = buttons[1];
        serialized.FindProperty("splitButton").objectReferenceValue = buttons[2];
        serialized.ApplyModifiedProperties();

        menu.SetActive(false);

        Selection.activeGameObject = menu;

        Debug.Log("Inventory context menu created.");
    }

    private static void CreateButton(
        Transform parent,
        string text,
        float y)
    {
        var go = new GameObject(
            text,
            typeof(RectTransform),
            typeof(Image),
            typeof(Button));

        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 35);
        rect.anchoredPosition = new Vector2(0, y);

        var label = new GameObject(
            "Text",
            typeof(RectTransform),
            typeof(TextMeshProUGUI));

        label.transform.SetParent(go.transform, false);

        var textComponent = label.GetComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.fontSize = 18;

        var labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
    }
}
