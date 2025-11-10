using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    void Update()
    {
        // Get input from keyboard
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down
        
        // Move in direction player is facing
        Vector3 movement = transform.forward * vertical + transform.right * horizontal;
        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}
