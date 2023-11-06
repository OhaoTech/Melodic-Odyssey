using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private float speed; // Character movement speed
    [SerializeField] private float sprintMultiplier = 2f; // Multiplier for sprinting speed
    [SerializeField] private float firstJumpForce; // Force of the first jump
    [SerializeField] private float secondJumpForce; // Force of the second jump
    [SerializeField] private float glideForce = 0.5f; // Additional upward force during gliding
    [SerializeField] private float glideDuration = 1.0f; // Gliding duration
    [SerializeField] private float fallMultiplier = 2.5f; // Gravity multiplier when falling
    [SerializeField] private float lowJumpMultiplier = 2f; // Gravity multiplier for low jumps
    private Rigidbody2D body; // Rigidbody2D component of the character
    private bool isGrounded = true; // Whether the character is on the ground
    private bool isGliding = false; // Whether the character is gliding
    private int jumpCount = 0; // Number of jumps made
    private float glideTimeLeft; // Remaining gliding time

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Handle horizontal movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float currentSpeed = speed;

        // Check if Shift is pressed and the character is grounded
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            currentSpeed *= sprintMultiplier; // Increase speed when sprinting
        }

        body.velocity = new Vector2(horizontalInput * currentSpeed, body.velocity.y);

        // Handle jump logic
        if (Input.GetKeyDown(KeyCode.W) && (isGrounded || jumpCount < 2))
        {
            float jumpForce = (jumpCount == 0) ? firstJumpForce : secondJumpForce;
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            isGrounded = false;
            isGliding = false; // Reset gliding state
            jumpCount++;
        }

        // Check if the jump peak has been reached and start gliding
        if (!isGrounded && !isGliding && body.velocity.y <= 0)
        {
            if (Input.GetKey(KeyCode.W))
            {
                isGliding = true;
                glideTimeLeft = glideDuration;
            }
        }
        // Handle gliding logic
        if (isGliding && glideTimeLeft > 0)
        {
            // Apply a small upward force to simulate gliding and gradually decrease it to simulate a falling trend
            body.velocity = new Vector2(body.velocity.x, Mathf.Max(body.velocity.y, -glideForce));
            glideTimeLeft -= Time.deltaTime;
        }

        // Add extra gravity effect when the character is falling
        if (!isGliding && body.velocity.y < 0)
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Reset jump and gliding state when the character collides with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isGliding = false;
            jumpCount = 0; // Reset jump count
        }
    }
}
