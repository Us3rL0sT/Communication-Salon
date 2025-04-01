using UnityEngine;

public class InventoryDebugger : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;

    void Start()
    {
        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI не присвоен!");
            return;
        }

        Debug.Log("Тест InventoryUI:");
        Debug.Log($"Размер инвентаря: {inventoryUI.GetInventorySize()}");
        Debug.Log($"Свободный слот: {inventoryUI.GetFreeSlot()}");
    }
}