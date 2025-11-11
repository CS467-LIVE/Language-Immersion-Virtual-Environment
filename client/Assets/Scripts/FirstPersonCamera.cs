using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    public float mouseSensitivity = 6f;
    public float verticalLookLimit = 80f;
    
    private float rotationX = 0f;
    private Transform playerBody;
    
    void Start()
    {
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Get player transform
        playerBody = transform.parent;
    }
    
    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotate camera up/down (X axis)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -verticalLookLimit, verticalLookLimit);
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        
        // Rotate player left/right (Y axis)
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        // Relock cursor when clicking back into game
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
