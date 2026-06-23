using UnityEngine;

public class InputManager: MonoBehaviour
{
    public static InputManager Instance;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private bool jumpPressed;

    private float swipeThreshold = 50f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Unity Testing Inputs
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;

            DetectSwipe();

            jumpPressed = true;
        }

        // Mobile Touch Inputs
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;

                DetectSwipe();

                jumpPressed = true;
            }
        }
    }

    void DetectSwipe()
    {
        float deltaX = endTouchPosition.x - startTouchPosition.x;

        if (Mathf.Abs(deltaX) > swipeThreshold)
        {
            if (deltaX > 0)
            {
                GameManager.Instance.PlayerTurnRight();
            }
            else
            {
                GameManager.Instance.PlayerTurnLeft();
            }
        }
    }

    public bool JumpPressed()
    {
        if (jumpPressed)
        {
            jumpPressed = false;
            return true;

        }

        return false;
    }
}
