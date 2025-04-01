using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Button[] inventorySlots;

    void Start()
    {
        // При старте очищаем все слоты
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            ClearSlot(i);
        }
    }

    public void AddItem(Sprite itemSprite, int slotIndex)
    {
        Debug.Log($"Попытка добавить предмет в слот {slotIndex}");

        if (slotIndex < 0 || slotIndex >= inventorySlots.Length)
        {
            Debug.LogError($"Неверный индекс слота: {slotIndex}");
            return;
        }

        inventorySlots[slotIndex].image.sprite = itemSprite;
        inventorySlots[slotIndex].image.enabled = true;
        Debug.Log($"Предмет добавлен в слот {slotIndex}");
    }

    public void ClearSlot(int slotIndex)
    {
        Debug.Log($"Очищаем слот {slotIndex}");

        if (slotIndex < 0 || slotIndex >= inventorySlots.Length) return;
        inventorySlots[slotIndex].image.sprite = null;
        inventorySlots[slotIndex].image.enabled = false;
    }

    public int GetFreeSlot()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Слот свободен, если в нем нет спрайта ИЛИ он отключен
            if (!inventorySlots[i].image.enabled || inventorySlots[i].image.sprite == null)
                return i;
        }
        return -1;
    }

    public int GetInventorySize()
    {
        return inventorySlots.Length;
    }

    public Sprite GetItemSprite(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length) return null;
        return inventorySlots[slotIndex].image.sprite;
    }
}