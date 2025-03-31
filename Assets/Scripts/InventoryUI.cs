using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Button[] inventorySlots;

    public void AddItem(Sprite itemSprite, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length) return;
        inventorySlots[slotIndex].image.sprite = itemSprite;
        inventorySlots[slotIndex].image.enabled = true;
    }

    public void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length) return;
        inventorySlots[slotIndex].image.sprite = null;
        inventorySlots[slotIndex].image.enabled = false;
    }

    public int GetFreeSlot()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].image.sprite == null)
                return i;
        }
        return -1;
    }
    // Возвращает размер инвентаря
    public int GetInventorySize()
    {
        return inventorySlots.Length;
    }

    // Возвращает спрайт в указанном слоте
    public Sprite GetItemSprite(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length) return null;
        return inventorySlots[slotIndex].image.sprite;
    }
}
