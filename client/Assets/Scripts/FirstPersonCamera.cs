using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Camera Rotation Settings")]
    public float rotationSpeed = 120f;
    public float verticalLookLimit = 80f;

    private float rotationX = 0f;
    private Transform playerBody;

    void Start()
    {
        // Keep cursor visible and unlocked for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Get player transform
        playerBody = transform.parent;
    }

    void Update()
    {
        // Get input for camera rotation
        float horizontalRotation = Input.GetAxis("Horizontal"); // A/D keys
        float verticalRotation = 0f;

        // Arrow keys for looking up/down
        if (Input.GetKey(KeyCode.UpArrow))
        {
            verticalRotation = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            verticalRotation = -1f;
        }

        // Apply rotation
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Rotate camera up/down (X axis) with arrow keys
        rotationX += verticalRotation * rotationAmount;
        rotationX = Mathf.Clamp(rotationX, -verticalLookLimit, verticalLookLimit);
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Rotate player left/right (Y axis) with A/D keys
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * horizontalRotation * rotationAmount);
        }
    }
}
