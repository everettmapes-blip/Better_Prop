using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;
    public bool isRunning = false;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        float speed = walkSpeed;

        if (isCrouching)
            speed = crouchSpeed;
        else if (isRunning)
            speed = runSpeed;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = (forward * moveZ + right * moveX) * speed;

        if (characterController.isGrounded)
        {
            if (moveDirection.y < 0)
                moveDirection.y = -2f; // keeps player grounded

            if (Input.GetButtonDown("Jump"))
                moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        moveDirection.x = move.x;
        moveDirection.z = move.z;

        // Crouch
        characterController.height = isCrouching ? crouchHeight : defaultHeight;
        characterController.center = new Vector3(0, characterController.height / 2f, 0);

        characterController.Move(moveDirection * Time.deltaTime);

        // Mouse look
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * lookSpeed);
    }
}