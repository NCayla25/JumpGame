using UnityEngine;

public class ScrollingObject : MonoBehaviour
{
    [Header("Despawn")]
    [SerializeField] private float despawnDistance = 14f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private Vector2 movementDirection = Vector2.up;
    private bool movementDirectionSet;

    private void Start()
    {
        if (!movementDirectionSet && GameFlow.Instance != null)
        {
            SetMovementDirection(GameFlow.Instance.DirectionVector);
        }
    }

    private void Update()
    {
        if (GameFlow.Instance == null)
            return;

        if (!GameFlow.Instance.IsRunning || GameFlow.Instance.IsGameOver)
            return;

        float speed = GameFlow.Instance.ScrollSpeed;

        if (showDebugLogs)
        {
            Debug.Log(
                $"{name} | Speed: {speed:F2} | " +
                $"Multiplier: {GameFlow.Instance.ScrollSpeedMultiplier:F2} | " +
                $"Position: {transform.position}"
                );
        }

        if (speed <= 0.001f)
            return;

        transform.position += (Vector3)(-movementDirection * speed * Time.deltaTime);

        TryDespawn();
    }

    public void SetMovementDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.001f)
            direction = Vector2.up;

        movementDirection = direction.normalized;
        movementDirectionSet = true;
    }

    private void TryDespawn()
    {
        Vector2 position = transform.position;

        bool isBehindPlayer = Vector2.Dot(position, -movementDirection) > despawnDistance;
        bool isFarAway = position.magnitude > despawnDistance * 2f;

        if (isBehindPlayer || isFarAway)
        {
            Destroy(gameObject);
        }
    }
}
