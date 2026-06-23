using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 10f;

    private Rigidbody2D rb;

    private bool isGrounded = false;

    public enum Direction
    {
        Up,
        Left,
        Right
    }

    public Direction currentDirection = Direction.Up;

    private Vector2 moveDirection = Vector2.up;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateMoveDirection();
    }

    void Update()
    {
        Move();

        if (InputManager.Instance.JumpPressed())
        {
            Jump();
        }
    }

    void Move()
    {
        Vector2 movement = moveDirection * moveSpeed;

        rb.linearVelocity = new Vector2(
            movement.x,
            rb.linearVelocity.y
        );

        if (currentDirection == Direction.Up)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (!isGrounded)
        {
            return;
        }

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );
    }

    public void TurnLeft()
    {
        currentDirection = Direction.Left;
        UpdateMoveDirection();
    }

    public void TurnRight()
    {
        currentDirection = Direction.Right;
        UpdateMoveDirection();
    }

    public void ContinueForward()
    {
        currentDirection = Direction.Up;
        UpdateMoveDirection();
    }

    void UpdateMoveDirection()
    {
        switch (currentDirection)
        {
            case Direction.Up:
                moveDirection = Vector2.up;
                break;

            case Direction.Left:
                moveDirection = Vector2.left;
                break;

            case Direction.Right:
                moveDirection = Vector2.right;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
