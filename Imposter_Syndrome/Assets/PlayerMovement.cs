using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    // Adjustable movement variables
    public float playerSpeed = 5.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f; // Standard Earth gravity

    private void Start()
    {
        // Get the CharacterController component attached to the GameObject
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if the character is grounded
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            // Reset vertical velocity if grounded to prevent gravity from building up
            playerVelocity.y = 0f;
        }

        // Read input for horizontal and vertical axes (WASD/Arrow Keys)
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Move the character relative to its current forward direction
        // For simple character movement, it's often best to handle rotation separately or adjust this line for camera-relative movement
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Optional: Make the character face the direction of movement
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Handle jumping
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            // Apply upward velocity based on jump height and gravity
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        // Apply gravity every frame
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Apply the final vertical movement (gravity and jump)
        controller.Move(playerVelocity * Time.deltaTime);
    }
}