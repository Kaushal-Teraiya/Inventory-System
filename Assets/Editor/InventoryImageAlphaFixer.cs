using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public sealed class InventoryImageAlphaFixer : EditorWindow
{
    [SerializeField] private GameObject inventoryUI;

    [MenuItem("Tools/Inventory/Fix Image Alpha")]
    private static void Open()
    {
        GetWindow<InventoryImageAlphaFixer>("Image Alpha Fixer");
    }

    private void OnGUI()
    {
        inventoryUI = (GameObject)EditorGUILayout.ObjectField(
            "Inventory UI Root", inventoryUI, typeof(GameObject), true);

        using (new EditorGUI.DisabledScope(inventoryUI == null))
        {
            if (GUILayout.Button("Set All Image Alpha to 1"))
                FixAlpha();
        }
    }

    private void FixAlpha()
    {
        var images = inventoryUI.GetComponentsInChildren<Image>(true);
        Undo.RecordObjects(images, "Fix Inventory Image Alpha");

        foreach (var image in images)
        {
            Color color = image.color;
            color.a = 1f;
            image.color = color;
            EditorUtility.SetDirty(image);
        }

        Debug.Log($"Fixed alpha on {images.Length} Images.", inventoryUI);
    }
}
