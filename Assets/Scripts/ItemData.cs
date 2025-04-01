using UnityEngine;

public enum ItemType { Product, Scanner, Other }
public enum ProductCategory { Food, Electronics, Clothing, Items }

public class ItemData : MonoBehaviour
{
    public string itemName = "Товар";
    public float price = 100f;
    public ItemType type; // Используем enum вместо bool
    public bool isScannable = true;
    public string barcode;
    public ProductCategory category;

    // Для обратной совместимости (если где-то используется)
    public bool IsProduct => type == ItemType.Product;
}