using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed; // Character movement speed
    [SerializeField] private float sprintMultiplier = 2f; // Multiplier for sprinting speed
    [SerializeField] private float firstJumpForce; // Force of the first jump
    [SerializeField] private float secondJumpForce; // Force of the second jump
    [SerializeField] private float glideForce = 0.5f; // Additional upward force during gliding
    [SerializeField] private float glideDuration = 1.0f; // Gliding duration
    [SerializeField] private float fallMultiplier = 2.5f; // Gravity multiplier when falling
    [SerializeField] private float lowJumpMultiplier = 2f; // Gravity multiplier for low jumps
    [SerializeField] private float airControlFactor = 0.5f; // Responsiveness of movement in the air
    private Rigidbody2D body; // Rigidbody2D component of the character
    private bool isGrounded = true; // Whether the character is on the ground
    private bool isGliding = false; // Whether the character is gliding
    private int jumpCount = 0; // Number of jumps made
    private float glideTimeLeft; // Remaining gliding time

    private bool isSprinting = false; // Added variable to keep track of sprinting state


    public float score;
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Horizontal Movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            currentSpeed *= sprintMultiplier;
            isSprinting = true; // Set sprinting state when sprinting starts
        }
        else if (isGrounded)
        {
            isSprinting = false; // Reset sprinting state when not sprinting
        }

        float horizontalVelocity;

        if (!isGrounded)
        {
            // Apply sprint multiplier if sprinting was active at the start of the jump
            float airSpeed = isSprinting ? speed * sprintMultiplier : speed;
            horizontalVelocity = Mathf.Lerp(body.velocity.x, horizontalInput * airSpeed, airControlFactor);
        }
        else
        {
            horizontalVelocity = horizontalInput * currentSpeed;
        }


        // Vertical Movement
        if (Input.GetKeyDown(KeyCode.W) && (isGrounded || jumpCount < 2))
        {
            float jumpForce = (jumpCount == 0) ? firstJumpForce : secondJumpForce;
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            isGrounded = false;
            jumpCount++;
        }

        // Apply Movement
        body.velocity = new Vector2(horizontalVelocity, body.velocity.y);

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
        // Check the collision angle to see if it's below a certain threshold, indicating a "ground" collision
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Optionally, check if the player has left the ground
        isGrounded = false;
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Note")
        {
            NoteManager noteManager = collision.GetComponent<NoteManager>();
            if (noteManager != null)
            {
                // 增加分数
                score += noteManager.scoreValue;

                // 消费音符（播放淡出动画并销毁）
                noteManager.ConsumeNote();
            }
        }
    }


}
