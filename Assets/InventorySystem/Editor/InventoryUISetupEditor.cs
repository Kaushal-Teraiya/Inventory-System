using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class InventoryUISetupEditor
{
    [MenuItem("Inventory System/Setup Inventory Slots")]
    public static void Setup()
    {
        var root = GameObject.Find("InventoryUI");

        if (root == null)
        {
            Debug.LogError("InventoryUI not found.");
            return;
        }

        EnsureEventSystem();

        foreach (var transform in root.GetComponentsInChildren<Transform>(true))
        {
            if (!transform.name.StartsWith("Slot_"))
                continue;

            var image = transform.GetComponent<Image>();
            var button = transform.GetComponent<Button>();

            if (image == null)
                image = transform.gameObject.AddComponent<Image>();

            if (button == null)
                button = transform.gameObject.AddComponent<Button>();

            image.raycastTarget = true;
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;

            var colors = button.colors;
            colors.normalColor = new Color(0.16f, 0.16f, 0.16f, 1f);
            colors.highlightedColor = Color.yellow;
            colors.pressedColor = Color.green;
            colors.selectedColor = Color.yellow;
            colors.disabledColor = Color.gray;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;

            var icon = transform.Find("Icon");
            if (icon != null)
            {
                var iconImage = icon.GetComponent<Image>();
                if (iconImage != null)
                    iconImage.raycastTarget = false;
            }

            var quantity = transform.Find("Quantity");
            if (quantity != null)
            {
                var text = quantity.GetComponent<TMPro.TMP_Text>();
                if (text != null)
                    text.raycastTarget = false;
            }
        }

        EditorUtility.SetDirty(root);

        Debug.Log("Inventory slot button diagnostics configured.");
    }

    private static void EnsureEventSystem()
    {
        var eventSystem = Object.FindFirstObjectByType<EventSystem>();

        if (eventSystem == null)
        {
            var go = new GameObject(
                "EventSystem",
                typeof(EventSystem));

            eventSystem = go.GetComponent<EventSystem>();
            Undo.RegisterCreatedObjectUndo(go, "Create EventSystem");
        }

        var inputModule =
            eventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        if (inputModule == null)
            eventSystem.gameObject.AddComponent<
                UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
    }
}
