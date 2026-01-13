using UnityEngine;

public class CameraFollow : MonoBehaviour
{
        public Transform playerTarget; // Assign your player's head/body here
        public Vector3 offset = new Vector3(0f, 0f, 0f); // Camera position relative to player
        public float smoothSpeed = 0.125f; // Smoothing factor

        void LateUpdate() // Use LateUpdate for camera movements
        {
            if (playerTarget != null)
            {
                Vector3 desiredPosition = playerTarget.position + playerTarget.rotation * offset;
                transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.LookAt(playerTarget); // Makes the camera look at the target
            }
        }
    
}
