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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerBody = transform.parent;
    }

    void Update()
    {
        // Left/Right arrow keys for horizontal rotation
        float horizontalRotation = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) horizontalRotation = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontalRotation = 1f;

        // Up/Down arrow keys for vertical camera look
        float verticalRotation = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) verticalRotation = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) verticalRotation = -1f;

        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Apply vertical rotation to camera
        rotationX += verticalRotation * rotationAmount;
        rotationX = Mathf.Clamp(rotationX, -verticalLookLimit, verticalLookLimit);
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Apply horizontal rotation to player body
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * horizontalRotation * rotationAmount);
    }
}
