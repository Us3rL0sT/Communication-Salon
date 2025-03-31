using System.Collections;
using UnityEngine;

public class WickUpWeapon : MonoBehaviour
{
    public GameObject playerCamera;
    public float distance = 15f;
    private GameObject currentWeapon;
    private bool canPickUp;
    private Coroutine rotationCoroutine;

    [SerializeField] private InventoryUI inventoryUI; // Ссылка на инвентарь
    [SerializeField] private Sprite weaponSprite; // Иконка оружия для инвентаря


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) PickUp();
        if (Input.GetKeyDown(KeyCode.Q)) Drop();

        if (Input.GetKeyDown(KeyCode.R))
        {
            float angle = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 90f : 45f;
            StartRotation(angle);
        }
    }

    void PickUp()
    {
        // Бросаем луч из камеры
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, distance))
        {
            // Если попали в объект с тегом "Pickup"
            if (hit.transform.CompareTag("Pickup"))
            {
                // Если уже что-то держим - сначала выкидываем
                if (canPickUp) Drop();

                // Берем новый предмет
                currentWeapon = hit.transform.gameObject;
                currentWeapon.GetComponent<Rigidbody>().isKinematic = true;
                currentWeapon.transform.parent = transform;
                currentWeapon.transform.localPosition = Vector3.zero;
                currentWeapon.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
                canPickUp = true;

                // Добавляем в инвентарь
                int freeSlot = inventoryUI.GetFreeSlot();
                if (freeSlot != -1)
                {
                    inventoryUI.AddItem(weaponSprite, freeSlot);
                }
                else
                {
                    Debug.Log("Инвентарь полон!");
                }
            }
        }
    }

    void Drop()
    {
        if (currentWeapon == null) return;

        // Выкидываем предмет
        currentWeapon.transform.parent = null;
        currentWeapon.GetComponent<Rigidbody>().isKinematic = false;
        canPickUp = false;

        // Удаляем из инвентаря
        for (int i = 0; i < inventoryUI.GetInventorySize(); i++)
        {
            if (inventoryUI.GetItemSprite(i) == weaponSprite)
            {
                inventoryUI.ClearSlot(i);
                break;
            }
        }

        currentWeapon = null;
    }

    void StartRotation(float angle)
    {
        if (currentWeapon == null) return;

        if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
        rotationCoroutine = StartCoroutine(SmoothRotate(angle));
    }

    IEnumerator SmoothRotate(float angle)
    {
        Quaternion startRotation = currentWeapon.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, angle, 0f);
        float duration = 0.3f; // Время вращения
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            currentWeapon.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            yield return null;
        }

        currentWeapon.transform.rotation = targetRotation;
    }
}
