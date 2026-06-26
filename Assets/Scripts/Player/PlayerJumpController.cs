using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJumpController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visualRoot;

    [Header("Up Direction - Flappy Mode")]
    [SerializeField] private float flapVelocity = 7.5f;
    [SerializeField] private float upModeGravityScale = 2.4f;
    [SerializeField] private float maxFallSpeed = 9f;
    [SerializeField] private float maxRiseSpeed = 8.5f;
    [SerializeField] private float fixedXPosition = 0f;

    [Header("Up Direction - Screen Bounds")]
    [SerializeField] private float lowerDeathY = -5.4f;
    [SerializeField] private float ceilingY = 4.4f;
    [SerializeField] private bool killIfFallsBelowScreen = true;
    [SerializeField] private bool clampToCeiling = true;

    [Header("Left/Right Direction - Jump Over Mode")]
    [SerializeField] private float groundJumpDuration = 0.55f;
    [SerializeField] private float groundJumpHeight = 1.25f;
    [SerializeField] private float sideModeRecenteringSpeed = 12f;
    [SerializeField] private Vector2 sideModePlayerPosition = Vector2.zero;

    [Header("Optional Feel")]
    [SerializeField] private bool useSquashStretch = true;
    [SerializeField] private float maxStretch = 0.15f;

    public bool IsGroundJumping { get; private set; }

    public bool IsFlappyMode
    {
        get
        {
            return GameFlow.Instance != null &&
                GameFlow.Instance.CurrentDirection == TravelDirection.Up;
        }
    }

    private Rigidbody2D rb;

    private float groundJumpTimer;
    private Vector3 visualStartLocalPosition;
    private Vector3 visualStartScale;

    private bool inputSubscribed;
    private bool gameFlowSubscribed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        visualStartLocalPosition = visualRoot.localPosition;
        visualStartScale = visualRoot.localScale;
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void Start()
    {
        TrySubscribe();
        ApplyModeForCurrentDirection(resetVelocity: true);
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Update()
    {
        TrySubscribe();

        if (GameFlow.Instance != null && GameFlow.Instance.IsGameOver)
            return;

        UpdateGroundJumpVisual();
    }

    private void FixedUpdate()
    {
        if (GameFlow.Instance == null)
            return;

        if (GameFlow.Instance.IsGameOver)
        {
            FreezePlayer();
            return;
        }

        if (IsFlappyMode)
        {
            HandleFlappyFixedUpdate();
        }
        else
        {
            HandleSideModeFixedUpdate();
        }
    }

    private void TrySubscribe()
    {
        if (!inputSubscribed && PlayerInputReader.Instance != null)
        {
            PlayerInputReader.Instance.JumpPressed += OnJumpPressed;
            inputSubscribed = true;
        }

        if (!gameFlowSubscribed && GameFlow.Instance != null)
        {
            GameFlow.Instance.DirectionChanged += OnDirectionChanged;
            gameFlowSubscribed = true;
        }
    }

    private void Unsubscribe()
    {
        if (inputSubscribed && PlayerInputReader.Instance != null)
        {
            PlayerInputReader.Instance.JumpPressed -= OnJumpPressed;
        }

        if (gameFlowSubscribed && GameFlow.Instance != null)
        {
            GameFlow.Instance.DirectionChanged -= OnDirectionChanged;
        }

        inputSubscribed = false;
        gameFlowSubscribed = false;
    }

    private void OnDirectionChanged(TravelDirection oldDirection, TravelDirection newDirection)
    {
        ResetGroundJumpVisual();
        ApplyModeForCurrentDirection(resetVelocity: true);
    }

    private void ApplyModeForCurrentDirection(bool resetVelocity)
    {
        if (GameFlow.Instance == null)
            return;

        if (GameFlow.Instance.CurrentDirection == TravelDirection.Up)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = upModeGravityScale;

            if (resetVelocity)
                rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnJumpPressed()
    {
        if (GameFlow.Instance == null || GameFlow.Instance.IsGameOver)
            return;

        if (IsFlappyMode)
        {
            Flap();
        }
        else
        {
            TryGroundJump();
        }
    }

    private void Flap()
    {
        if (rb.bodyType != RigidbodyType2D.Dynamic)
            ApplyModeForCurrentDirection(resetVelocity: false);

        rb.linearVelocity = new Vector2(0f, flapVelocity);
    }

    private void TryGroundJump()
    {
        if (IsGroundJumping)
            return;

        IsGroundJumping = true;
        groundJumpTimer = 0f;
    }

    private void UpdateGroundJumpVisual()
    {
        if (!IsGroundJumping)
            return;

        groundJumpTimer += Time.deltaTime;

        float normalizedTime = Mathf.Clamp01(groundJumpTimer / groundJumpDuration);
        float arc = Mathf.Sin(normalizedTime * Mathf.PI);
        float height = arc * groundJumpHeight;

        visualRoot.localPosition = visualStartLocalPosition + Vector3.up * height;

        if (useSquashStretch)
        {
            float stretch = arc * maxStretch;

            visualRoot.localScale = new Vector3(

                visualStartScale.x - stretch * 0.5f,
                visualStartScale.y + stretch,
                visualStartScale.z
                );
        }

        if (normalizedTime >= 1f)
            ResetGroundJumpVisual();
    }

    private void ResetGroundJumpVisual()
    {
        IsGroundJumping = false;
        groundJumpTimer = 0f;

        if (visualRoot != null)
        {
            visualRoot.localPosition = visualStartLocalPosition;
            visualRoot.localScale = visualStartScale;
        }
    }

    private void HandleFlappyFixedUpdate()
    {
        if (rb.bodyType != RigidbodyType2D.Dynamic)
            ApplyModeForCurrentDirection(resetVelocity: false);

        Vector2 velocity = rb.linearVelocity;

        velocity.x = 0f;
        velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, maxRiseSpeed);

        rb.linearVelocity = velocity;

        Vector2 position = rb.position;
        position.x = fixedXPosition;

        if (clampToCeiling && position.y > ceilingY)
        {
            position.y = ceilingY;

            if (rb.linearVelocity.y > 0f)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }

        rb.position = position;

        if (killIfFallsBelowScreen && rb.position.y < lowerDeathY)
        {
            GameFlow.Instance.GameOver();
        }
    }

    private void HandleSideModeFixedUpdate()
    {
        if (rb.bodyType != RigidbodyType2D.Kinematic)
            ApplyModeForCurrentDirection(resetVelocity: true);

        rb.linearVelocity = Vector2.zero;

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            sideModePlayerPosition,
            sideModeRecenteringSpeed * Time.deltaTime
            );

        rb.MovePosition(nextPosition);
    }

    private void FreezePlayer()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
