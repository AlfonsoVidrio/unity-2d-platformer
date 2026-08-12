using UnityEngine;

public class Player : MonoBehaviour
{
    // =========================
    // MOVEMENT
    // =========================

    public float speed = 4f;
    public float jumpForce = 8f;


    // =========================
    // JUMP ASSIST
    // =========================

    // How long we remember that the player
    // pressed the jump button before touching the ground.
    public float jumpBufferTime = 0.1f;

    // How long the player can still jump
    // after leaving the ground.
    public float coyoteTime = 0.1f;


    // =========================
    // GROUND CHECK
    // =========================


    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;


    // =========================
    // INTERNAL VARIABLES
    // =========================

    private Rigidbody2D rb2D;

    // Stores the horizontal movement input.
    private float move;

    // Stores how much time is left for the jump buffer.
    private float jumpBufferCounter;

    // Stores how much time is left for coyote time.
    private float coyoteTimeCounter;

    // Indicates whether the player is touching the ground.
    private bool isGrounded;

    // Stores whether the player pressed the jump button.
    private bool jumpPressed;


    // =========================
    // INITIALIZATION
    // =========================

    void Start()
    {
        // Get the Rigidbody2D component from the player.
        rb2D = GetComponent<Rigidbody2D>();
    }


    // =========================
    // INPUT
    // =========================

    void Update()
    {
        // Read horizontal movement.
        // -1 = left
        //  0 = no movement
        //  1 = right
        move = Input.GetAxisRaw("Horizontal");


        // -------------------------
        // DETECT JUMP
        // -------------------------

        // GetButtonDown is true only on the frame
        // when the button is pressed.
        if (Input.GetButtonDown("Jump"))
        {
            // Remember that the player wants to jump.
            jumpPressed = true;

            // Start the jump buffer timer.
            jumpBufferCounter = jumpBufferTime;
        }


        // -------------------------
        // REDUCE JUMP BUFFER
        // -------------------------

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }


    // =========================
    // PHYSICS
    // =========================

    void FixedUpdate()
    {
        // -------------------------
        // CHECK IF GROUNDED
        // -------------------------

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );


        // -------------------------
        // COYOTE TIME
        // -------------------------

        if (isGrounded)
        {
            // If we are on the ground,
            // reset the coyote time counter.
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            // If we are in the air,
            // decrease the remaining coyote time.
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }


        // -------------------------
        // HORIZONTAL MOVEMENT
        // -------------------------

        rb2D.linearVelocity = new Vector2(
            move * speed,
            rb2D.linearVelocity.y
        );


        // -------------------------
        // JUMP
        // -------------------------

        // We can jump if:
        //
        // 1. The player pressed jump.
        // 2. The jump input is still inside the jump buffer.
        // 3. The player is grounded or still has coyote time.
        if (
            jumpPressed &&
            jumpBufferCounter > 0 &&
            coyoteTimeCounter > 0
        )
        {
            // Apply the vertical jump velocity.
            rb2D.linearVelocity = new Vector2(
                rb2D.linearVelocity.x,
                jumpForce
            );


            // Consume the jump input.
            jumpPressed = false;

            // Consume the jump buffer.
            jumpBufferCounter = 0;

            // Consume the coyote time.
            coyoteTimeCounter = 0;
        }
    }
}