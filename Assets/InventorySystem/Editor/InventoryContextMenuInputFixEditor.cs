using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class InventoryContextMenuInputFixEditor
{
    [MenuItem("Inventory System/Fix Context Menu Input")]
    public static void Fix()
    {
        var menu = GameObject.Find("ContextMenu");

        if (menu == null)
        {
            Debug.LogError("ContextMenu not found.");
            return;
        }

        var background = menu.GetComponent<Image>();

        if (background != null)
            background.raycastTarget = false;

        foreach (var button in menu.GetComponentsInChildren<Button>(true))
        {
            var image = button.GetComponent<Image>();

            if (image != null)
                image.raycastTarget = true;

            button.interactable = true;
        }

        Debug.Log("Context menu button input fixed.");
    }
}
