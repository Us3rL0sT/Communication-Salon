using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WickUpWeapon : MonoBehaviour
{
    public GameObject playerCamera;
    public float distance = 15;
    GameObject currentWeapon;
    bool canPickUp;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) PickUp();
        if (Input.GetKeyDown(KeyCode.Q)) Drop();
    }

    void PickUp()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, distance))
        {
            if (hit.transform.tag == "Pickup")
            {
                if (canPickUp) Drop();
                currentWeapon = hit.transform.gameObject;
                currentWeapon.GetComponent<Rigidbody>().isKinematic = true;
                currentWeapon.transform.parent = transform;
                currentWeapon.transform.localPosition = Vector3.zero;
                currentWeapon.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
                canPickUp = true;
            }
        }
    }

    void Drop()
    {
        currentWeapon.transform.parent = null;
        currentWeapon.GetComponent<Rigidbody>().isKinematic = false;
        canPickUp = false;
        currentWeapon = null;
    }
}