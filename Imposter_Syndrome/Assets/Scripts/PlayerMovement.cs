using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float originalWalkSpeed = 6f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;
    public bool isRunning = false;
    public bool isCrouching = false;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private float originalHeight;
    private Vector3 originalCenter;
    private bool canMove = true;

    private float originalCameraY;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalHeight = characterController.height;
        originalCenter = characterController.center;
        originalCameraY = playerCamera.transform.localPosition.y;
    }

    void Update()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // 1. INPUTS
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        
        // Fixed: added 'bool' type definition
        
        
        // Check for Running
        isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        // Set Speed
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        if (!isRunning) currentSpeed = originalWalkSpeed; 
        if (isCrouching) currentSpeed = crouchSpeed; // Override speed if crouching

        // 2. CROUCH LOGIC
        // Consolidating this here to avoid conflicts
        if (isCrouching)
        {
            characterController.height = crouchHeight;
            characterController.center = originalCenter - new Vector3(0, (originalHeight - crouchHeight) / 2f, 0);
            playerCamera.transform.localPosition = new Vector3(0, originalCameraY - 0.6f, 0);
        }
        else
        {
            characterController.height = originalHeight;
                characterController.center = originalCenter;
                playerCamera.transform.localPosition = new Vector3(0, originalCameraY, 0);
            // Check if we are forced to stay crouched (something above head)
            
        }

        // 3. MOVEMENT (ESDF REPLACEMENT)
        // We replace Input.GetAxis with manual Key Checks
        float moveX = 0;
        float moveZ = 0;

        // FORWARD / BACK
        if (Input.GetKey(KeyCode.E)) moveZ = 1;
        if (Input.GetKey(KeyCode.D)) moveZ = -1;

        // LEFT / RIGHT
        if (Input.GetKey(KeyCode.S)) moveX = -1;
        if (Input.GetKey(KeyCode.F)) moveX = 1;

        // Calculate movement vector
        Vector3 move = (forward * moveZ + right * moveX) * currentSpeed;

        // 4. GRAVITY AND JUMP
        if (characterController.isGrounded)
        {
            // Reset Y velocity when grounded
            moveDirection.y = -1f; 

            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpPower;
            }
        }
        else
        {
            // Apply Gravity
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Apply X and Z movement
        moveDirection.x = move.x;
        moveDirection.z = move.z;

        // Move the controller
        if (canMove)
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }

        // 5. CAMERA ROTATION
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * lookSpeed);
        }
    }
}