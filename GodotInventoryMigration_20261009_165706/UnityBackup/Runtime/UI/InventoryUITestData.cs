using InventorySystem.Domain;
using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem.UI
{
    public sealed class InventoryUITestData : MonoBehaviour
    {
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private ItemDefinition testItem;
        [SerializeField] private int quantity = 5;

        private void Start()
        {
            if (inventoryUI == null || testItem == null)
                return;

            inventoryUI.Inventory.AddItem(
                new ItemInstance(testItem),
                quantity);
        }
    }
}
