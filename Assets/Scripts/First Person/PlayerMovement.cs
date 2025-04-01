using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 75;
    [SerializeField] private float walkSpeed = 7;
    [SerializeField] private float runSpeed = 10;
    [SerializeField] private float jumpForce = 6;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip runSound;
    [SerializeField] private float crouchSpeed = 3.5f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchTransitionSpeed = 5f;

    // Параметры покачивания головой
    [SerializeField] private float walkBobbingSpeed = 14f;
    [SerializeField] private float runBobbingSpeed = 18f;
    [SerializeField] private float walkBobbingAmount = 0.05f;
    [SerializeField] private float runBobbingAmount = 0.1f;
    [SerializeField] private float crouchBobbingAmount = 0.025f;
    [SerializeField] private float bobbingResetSpeed = 2f;

    private CharacterController characterController;
    private Camera playerCamera;
    private Vector3 velocity;
    private Vector2 rotation;
    private Vector2 direction;
    private bool isMoving;
    private bool isCrouching;
    private float currentHeight;
    private float defaultCameraYPos;
    private float timer = 0f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        currentHeight = standingHeight;
        defaultCameraYPos = playerCamera.transform.localPosition.y;
    }

    private void Update()
    {
        characterController.Move(velocity * Time.deltaTime);
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (characterController.isGrounded)
            velocity.y = Input.GetKeyDown(KeyCode.Space) ? jumpForce : -0.1f;
        else
            velocity.y += gravity * Time.deltaTime;

        mouseDelta *= rotateSpeed * Time.deltaTime;
        rotation.y += mouseDelta.x;
        rotation.x = Mathf.Clamp(rotation.x - mouseDelta.y, -90, 90);
        playerCamera.transform.localEulerAngles = rotation;

        HandleCrouch();
        HandleFootsteps();
        HandleHeadBobbing();
    }

    private void FixedUpdate()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        if (isCrouching)
            speed = crouchSpeed;

        direction *= speed;
        Vector3 move = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
        velocity = new Vector3(move.x, velocity.y, move.z);
    }

    private void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.X))
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }

        currentHeight = Mathf.Lerp(currentHeight, isCrouching ? crouchHeight : standingHeight, Time.deltaTime * crouchTransitionSpeed);
        characterController.height = currentHeight;
    }

    private void HandleFootsteps()
    {
        if (characterController.isGrounded && direction.magnitude > 0)
        {
            if (!isMoving)
            {
                isMoving = true;
                footstepAudio.clip = Input.GetKey(KeyCode.LeftShift) ? runSound : walkSound;
                footstepAudio.loop = true;
                footstepAudio.Play();
            }
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
                footstepAudio.Stop();
            }
        }
    }

    private void HandleHeadBobbing()
    {
        if (!characterController.isGrounded) return;

        float waveslice = 0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            // Сброс покачивания, когда игрок не двигается
            timer = 0f;
            Vector3 cameraPos = playerCamera.transform.localPosition;
            cameraPos.y = Mathf.Lerp(cameraPos.y, defaultCameraYPos, Time.deltaTime * bobbingResetSpeed);
            playerCamera.transform.localPosition = cameraPos;
        }
        else
        {
            // Вычисление покачивания
            float bobbingSpeed = isCrouching ? walkBobbingSpeed * 0.5f :
                                Input.GetKey(KeyCode.LeftShift) ? runBobbingSpeed : walkBobbingSpeed;

            timer += bobbingSpeed * Time.deltaTime;
            waveslice = Mathf.Sin(timer);

            float bobbingAmount = isCrouching ? crouchBobbingAmount :
                                 Input.GetKey(KeyCode.LeftShift) ? runBobbingAmount : walkBobbingAmount;

            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Clamp(Mathf.Abs(horizontal) + Mathf.Abs(vertical), 0f, 1f);
            translateChange = totalAxes * translateChange;

            Vector3 cameraPos = playerCamera.transform.localPosition;
            cameraPos.y = defaultCameraYPos + translateChange;
            playerCamera.transform.localPosition = cameraPos;
        }
    }
}