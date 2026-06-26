using UnityEngine;

public class ScrollingObject : MonoBehaviour
{
    [Header("Despawn")]
    [SerializeField] private float despawnDistance = 14f;

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
