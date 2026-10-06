using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public static class InventoryUIInputFixEditor
{
    [MenuItem("Inventory System/Fix UI Input")]
    public static void Fix()
    {
        var canvas = Object.FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("No Canvas found in the scene.");
            return;
        }

        var raycaster = canvas.GetComponent<GraphicRaycaster>();

        if (raycaster == null)
            canvas.gameObject.AddComponent<GraphicRaycaster>();

        var eventSystem = Object.FindFirstObjectByType<EventSystem>();

        if (eventSystem == null)
        {
            var go = new GameObject(
                "EventSystem",
                typeof(EventSystem));

            eventSystem = go.GetComponent<EventSystem>();

            Undo.RegisterCreatedObjectUndo(
                go,
                "Create EventSystem");
        }

        var oldModule =
            eventSystem.GetComponent<StandaloneInputModule>();

        if (oldModule != null)
            Object.DestroyImmediate(oldModule);

        var inputModule =
            eventSystem.GetComponent<InputSystemUIInputModule>();

        if (inputModule == null)
        {
            inputModule =
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
        }

        Debug.Log(
            "UI input fixed: Canvas Raycaster + EventSystem + InputSystemUIInputModule.");

        Selection.activeGameObject = eventSystem.gameObject;
    }
}
