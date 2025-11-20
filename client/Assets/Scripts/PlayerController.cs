using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.5f;    // Speed of movement
    public float jumpHeight = 1f;   // Height of the jump
    public float gravity = -14f;  // How strong gravity is

    private CharacterController controller;
    private Vector3 velocity;   // Tracks vertical movement
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if grounded
        bool isGrounded = controller.isGrounded;

        // Reset falling velocity when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to stay grounded
        }

        // Get input from keyboard
        float vertical = Input.GetAxis("Vertical");     // W/S

        // Move in direction player is facing
        Vector3 movement = transform.forward * vertical;

        // CharacterController for collisions
        controller.Move(movement * moveSpeed * Time.deltaTime);

        // Jump
        // When spacebar pressed and on ground
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Calculate jump velocity
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Always apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
