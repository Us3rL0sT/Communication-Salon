using UnityEngine;
using System.Collections;
public class PickUpWeapon : MonoBehaviour
{
    public GameObject playerCamera;
    public float distance = 15f;
    private GameObject[] inventory = new GameObject[3];
    private int currentSlot = 0;

    public string itemName;
    public float price;
    public bool isScannable; // Можно ли отсканировать (для сканера=false)

    [Header("Zoom Settings")]
    [SerializeField] private float zoomDistance = 0.5f; // Дистанция приближения
    [SerializeField] private float zoomSpeed = 5f;      // Скорость приближения
    [SerializeField] private float rotationSpeed = 2f; // Скорость вращения при приближении

    private bool isZooming = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Sprite weaponSprite;


    void Start()
    {
        if (CashRegisterSystem.Instance == null)
            Debug.LogError("CashRegisterSystem НЕ найден на сцене!");
        else
            Debug.Log("CashRegisterSystem успешно загружен.");
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Нажата клавиша E - попытка поднять предмет");
            PickUp();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Нажата клавиша Q - попытка выбросить предмет");
            Drop();
        }

        // Переключение между слотами инвентаря
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Переключение на слот 1");
            SwitchToSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Переключение на слот 2");
            SwitchToSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Переключение на слот 3");
            SwitchToSlot(2);
        }

        if (Input.GetKeyDown(KeyCode.R) && inventory[currentSlot] != null)
        {
            Debug.Log("Поворот предмета в слоте " + currentSlot);
            float angle = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 90f : 45f;
            inventory[currentSlot].GetComponent<WeaponRotator>().StartRotation(angle);
        }

        if (Input.GetKeyDown(KeyCode.C)) CombineItems();

        if (Input.GetKeyDown(KeyCode.V)) ToggleZoom();

        if (isZooming) HandleZoom();
    }

    void ToggleZoom()
    {
        if (inventory[currentSlot] == null) return;

        isZooming = !isZooming;

        if (isZooming)
        {
            // Запоминаем исходную позицию
            originalPosition = inventory[currentSlot].transform.localPosition;
            originalRotation = inventory[currentSlot].transform.localRotation;
        }
        else
        {
            // Возвращаем на место
            StartCoroutine(ResetPosition());
        }
    }

    void HandleZoom()
    {
        if (inventory[currentSlot] == null) return;

        // Плавное перемещение к позиции приближения
        Transform itemTransform = inventory[currentSlot].transform;
        itemTransform.localPosition = Vector3.Lerp(
            itemTransform.localPosition,
            new Vector3(0, 0, zoomDistance),
            zoomSpeed * Time.deltaTime
        );

        // Вращение предмета
        itemTransform.Rotate(Vector3.up * rotationSpeed * 10 * Time.deltaTime, Space.World);
    }

    IEnumerator ResetPosition()
    {
        Transform itemTransform = inventory[currentSlot].transform;
        float duration = 0.5f;
        float elapsed = 0f;

        Vector3 startPos = itemTransform.localPosition;
        Quaternion startRot = itemTransform.localRotation;

        while (elapsed < duration)
        {
            itemTransform.localPosition = Vector3.Lerp(startPos, originalPosition, elapsed / duration);
            itemTransform.localRotation = Quaternion.Slerp(startRot, originalRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        itemTransform.localPosition = originalPosition;
        itemTransform.localRotation = originalRotation;
    }

    // Альтернативный вариант с LeanTween


    void CombineItems()
    {
        // Проверка наличия инвентаря
        if (inventory == null || inventoryUI == null)
        {
            Debug.LogError("Система инвентаря не инициализирована!");
            return;
        }

        // Поиск сканера и товара
        ItemData scanner = null;
        ItemData product = null;
        int productSlot = -1;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null) continue;

            var itemData = inventory[i].GetComponent<ItemData>();
            if (itemData == null) continue;

            if (itemData.type == ItemType.Scanner && scanner == null)
            {
                scanner = itemData;
            }
            else if (itemData.type == ItemType.Product && product == null)
            {
                product = itemData;
                productSlot = i;
            }
        }

        // Проверка перед сканированием
        if (scanner == null)
        {
            Debug.Log("Сканер не найден в инвентаре!");
            return;
        }

        if (product == null)
        {
            Debug.Log("Товар не найден в инвентаре!");
            return;
        }

        // Проверка системы кассы
        if (CashRegisterSystem.Instance == null)
        {
            Debug.LogError("Система кассы не найдена на сцене!");
            return;
        }

        // Сканирование товара
        try
        {
            CashRegisterSystem.Instance.ScanProduct(product);

            // Удаление товара из инвентаря
            Destroy(inventory[productSlot]);
            inventory[productSlot] = null;
            inventoryUI.ClearSlot(productSlot);

            Debug.Log($"Товар {product.itemName} успешно отсканирован!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка сканирования: {e.Message}");
        }
    }
    void SwitchToSlot(int slotIndex)
    {
        Debug.Log($"Попытка переключиться на слот {slotIndex}");

        if (slotIndex < 0 || slotIndex >= inventory.Length)
        {
            Debug.LogError($"Неверный индекс слота: {slotIndex}");
            return;
        }

        // Если в этом слоте есть предмет, делаем его текущим
        if (inventory[slotIndex] != null)
        {
            Debug.Log($"В слоте {slotIndex} есть предмет - переключаемся");

            // Скрываем предыдущий предмет
            if (inventory[currentSlot] != null)
            {
                Debug.Log($"Скрываем предмет в предыдущем слоте {currentSlot}");
                inventory[currentSlot].SetActive(false);
            }

            currentSlot = slotIndex;
            Debug.Log($"Активируем предмет в новом слоте {currentSlot}");
            inventory[currentSlot].SetActive(true);
        }
        else
        {
            Debug.Log($"В слоте {slotIndex} нет предмета");
        }
    }

    void PickUp()
    {
        Debug.Log("Попытка поднять предмет");

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, distance))
        {
            if (hit.transform.CompareTag("Pickup"))
            {
                int freeSlot = inventoryUI.GetFreeSlot();
                if (freeSlot == -1)
                {
                    Debug.Log("Инвентарь полон!");
                    return;
                }

                GameObject pickedItem = hit.transform.gameObject;
                pickedItem.GetComponent<Rigidbody>().isKinematic = true;
                pickedItem.transform.parent = transform;
                pickedItem.transform.localPosition = Vector3.zero;
                pickedItem.transform.localEulerAngles = new Vector3(10f, 0f, 0f);

                // Деактивируем все предметы кроме первого
                pickedItem.SetActive(freeSlot == 0);

                inventory[freeSlot] = pickedItem;
                inventoryUI.AddItem(weaponSprite, freeSlot);

                Debug.Log($"Предмет добавлен в слот {freeSlot}");
            }
        }
    }

    void Drop()
    {
        Debug.Log($"Попытка выбросить предмет из слота {currentSlot}");

        if (inventory[currentSlot] == null)
        {
            Debug.LogWarning($"В текущем слоте {currentSlot} нет предмета");
            return;
        }

        var rotator = inventory[currentSlot].GetComponent<WeaponRotator>();
        if (rotator != null)
        {
            Debug.Log("Удаляем компонент WeaponRotator");
            Destroy(rotator);
        }

        Debug.Log($"Выбрасываем объект: {inventory[currentSlot].name}");
        inventory[currentSlot].transform.parent = null;
        inventory[currentSlot].GetComponent<Rigidbody>().isKinematic = false;
        inventory[currentSlot].SetActive(true);

        Debug.Log($"Очищаем слот {currentSlot} в UI инвентаря");
        inventoryUI.ClearSlot(currentSlot);
        inventory[currentSlot] = null;

        Debug.Log("Поиск следующего непустого слота");
        // Попробуем найти следующий непустой слот
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)
            {
                Debug.Log($"Найден предмет в слоте {i} - переключаемся");
                SwitchToSlot(i);
                return;
            }
        }

        Debug.Log("В инвентаре больше нет предметов");
    }
}