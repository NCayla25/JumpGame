using UnityEngine;
using UnityEngine.Rendering;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Shared Spawn")]
    [SerializeField] private float spawnDistance = 10f;

    [Header("Up Direction - Jetpack Gates")]
    [SerializeField] private float jetMinSpawnSeconds = 1.6f;
    [SerializeField] private float jetMaxSpawnSeconds = 2.4f;
    [SerializeField] private float jetGapSize = 2.7f;
    [SerializeField] private float jetGateWidth = 8.5f;
    [SerializeField] private float jetBarThickness = 1.25f;
    [SerializeField] private float jetGapCenterMinY = -2.2f;
    [SerializeField] private float jetGapCenterMaxY = 2.2f;

    [Header("Left/Right Direction - Jump Obstacles")]
    [SerializeField] private float sideMinSpawnSeconds = 1.0f;
    [SerializeField] private float sideMaxSpawnSeconds = 1.8f;
    [SerializeField] private Vector3 sideObstacleScale = new Vector3(1.1f, 1.1f, 1f);
    [SerializeField] private float sideObstacleLateralRandomness = 0f;

    private float spawnTimer;
    private float nextSpawnTime;
    private TravelDirection lastDirection;

    private void Start()
    {
        if (GameFlow.Instance != null)
            lastDirection = GameFlow.Instance.CurrentDirection;

        PickNextSpawnTime();
    }

    private void Update()
    {
        if (GameFlow.Instance == null)
            return;

        if (!GameFlow.Instance.IsRunning || GameFlow.Instance.IsGameOver)
            return;

        if (GameFlow.Instance.CurrentDirection != lastDirection)
        {
            lastDirection = GameFlow.Instance.CurrentDirection;
            spawnTimer = 0f;
            PickNextSpawnTime();
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= nextSpawnTime)
        {
            SpawnForCurrentDirection();
            spawnTimer = 0f;
            PickNextSpawnTime();
        }
    }

    private void SpawnForCurrentDirection()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogWarning("ObstacleSpawner has no obstacle prefab assigned.");
            return;
        }

        if (GameFlow.Instance.CurrentDirection == TravelDirection.Up)
        {
            SpawnJetGate();
        }
        else
        {
            SpawnSideJumpObstacle();
        }
    }

    private void SpawnJetGate()
    {
        Vector2 movementDirection = Vector2.up;

        float gapCenterWhenAtPlayer = Random.Range(
            jetGapCenterMinY,
            jetGapCenterMaxY
            );

        float halfGap = jetGapSize * 0.5f;
        float halfThickness = jetBarThickness * 0.5f;

        Vector3 topBarPosition = new Vector3(
            0f,
            spawnDistance + gapCenterWhenAtPlayer + halfGap + halfThickness,
            0f
            );

        Vector3 bottomBarPosition = new Vector3(
            0f,
            spawnDistance + gapCenterWhenAtPlayer - halfGap - halfThickness,
            0f
            );

        Vector3 barScale = new Vector3(
            jetGateWidth,
            jetBarThickness,
            1f
            );

        CreateObstacle(topBarPosition, barScale, movementDirection);
        CreateObstacle(bottomBarPosition,  barScale, movementDirection);
    }

    private void SpawnSideJumpObstacle()
    {
        Vector2 movementDirection = GameFlow.Instance.DirectionVector;
        Vector2 perpendicular = new Vector2(-movementDirection.y, movementDirection.x);

        float lateralOffset = Random.Range(
            -sideObstacleLateralRandomness,
            sideObstacleLateralRandomness
            );

        Vector2 spawnPosition =
            movementDirection * spawnDistance +
            perpendicular * lateralOffset;

        CreateObstacle(spawnPosition, sideObstacleScale, movementDirection);
    }

    private void CreateObstacle(Vector3 position, Vector3 scale, Vector2 movementDirection)
    {
        GameObject obstacle = Instantiate(
            obstaclePrefab,
            position,
            Quaternion.identity
            );

        obstacle.transform.localScale = scale;

        Collider2D collider = obstacle.GetComponent<Collider2D>();

        if (collider != null)
            collider.isTrigger = true;
        else
            Debug.LogWarning("Spawned obstacle has no Collider2D");

        ScrollingObject scrollingObject = obstacle.GetComponent<ScrollingObject>();

        if(scrollingObject == null)
            scrollingObject = obstacle.AddComponent<ScrollingObject>();

        scrollingObject.SetMovementDirection(movementDirection);

        if (obstacle.GetComponent<Obstacle>() == null)
            obstacle.AddComponent<Obstacle>();
    }

    private void PickNextSpawnTime()
    {
        if (GameFlow.Instance != null &&
            GameFlow.Instance.CurrentDirection == TravelDirection.Up)
        {
            nextSpawnTime = Random.Range(
                jetMinSpawnSeconds,
                jetMaxSpawnSeconds
            );
        }
        else
        {
            nextSpawnTime = Random.Range(
                sideMinSpawnSeconds,
                sideMaxSpawnSeconds
                );
        }
    }
}
