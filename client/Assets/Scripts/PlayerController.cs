using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 100f;
    
    void Update()
    {
        // Get input from keyboard
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Move forward/backward
        transform.Translate(Vector3.forward * vertical * moveSpeed * Time.deltaTime);
        
        // Rotate left/right
        transform.Rotate(Vector3.up * horizontal * rotateSpeed * Time.deltaTime);
    }
}
