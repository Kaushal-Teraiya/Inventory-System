using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class InventoryContextMenuStyleEditor
{
    [MenuItem("Inventory System/Style Context Menu")]
    public static void Style()
    {
        var menu = GameObject.Find("ContextMenu");

        if (menu == null)
        {
            Debug.LogError("ContextMenu not found.");
            return;
        }

        var image = menu.GetComponent<Image>();

        if (image != null)
        {
            image.color = new Color(0.06f, 0.06f, 0.06f, 0.97f);
        }

        foreach (var button in menu.GetComponentsInChildren<Button>(true))
        {
            var buttonImage = button.GetComponent<Image>();

            if (buttonImage != null)
                buttonImage.color = new Color(0.12f, 0.12f, 0.12f, 1f);
        }

        EditorUtility.SetDirty(menu);

        Debug.Log("Context menu styled.");
    }
}
