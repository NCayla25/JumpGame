using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float verticalSpeed = 5f;
    public float horizontalSpeed = 6f;

    [Header("Jetpack / Jump")]
    public float jetpackJumpForce = 8f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.15f;

    private bool isGrounded;

    public MovementDirection mode;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool tapped = false;

        if (Mouse.current != null && 
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            tapped = true;
        }

        if (Touchscreen.current != null && 
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            tapped = true;
        }

        if (tapped)
        {
            JetPack();
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void FixedUpdate()
    {
        //ApplyForwardMovement();
    }

    private void ApplyForwardMovement()
    {
        Vector2 velocity = rb.linearVelocity;

        switch (mode)
        {
            case MovementDirection.HorizontalLeft:
                velocity.x = -horizontalSpeed;
                break;
            case MovementDirection.HorizontalRight:
                velocity.x = horizontalSpeed;
                break;
        }

        rb.linearVelocity = new Vector2(velocity.x, velocity.y);
    }

    private void Jump()
    {
        if (!isGrounded)
        {
            return;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jetpackJumpForce, ForceMode2D.Impulse);
    }

    private void JetPack()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jetpackJumpForce, ForceMode2D.Impulse);
    }

    public void SetMode(MovementDirection newMode)
    {
        mode = newMode;
    }
}
