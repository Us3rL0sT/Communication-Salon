using UnityEngine;
using System.Collections.Generic;
public class CashRegisterSystem : MonoBehaviour
{
    public static CashRegisterSystem Instance;

    public float totalSum;
    public List<ItemData> scannedProducts = new List<ItemData>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ScanProduct(ItemData product)
    {
        scannedProducts.Add(product);
        totalSum += product.price;
        Debug.Log($"Отсканирован: {product.itemName} за {product.price} руб.");
        Debug.Log($"Общая сумма: {totalSum} руб.");
    }

    public void CompletePurchase()
    {
        // Вызывается при завершении покупки
        Debug.Log($"Покупка завершена. Итого: {totalSum} руб.");
        scannedProducts.Clear();
        totalSum = 0f;
    }
}