using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Shared Spawn")]
    [SerializeField] private float spawnDistance = 10f;

    [Header("Up Direction - Jetpack Gates")]
    [SerializeField] private float firstJetGateAfterDistance = 8f;
    [SerializeField] private float jetDistanceBetweenGates = 7f;

    [SerializeField] private float jetGapSize = 3.2f;
    [SerializeField] private float jetGapCenterMinY = -2.2f;
    [SerializeField] private float jetGapCenterMaxY = 2.2f;

    [SerializeField] private Vector3 jetBarScale = new Vector3(2.2f, 0.75f, 1f);
    [SerializeField] private float jetHorizontalRange = 2.8f;
    [SerializeField] private float jetHorizontalSpeed = 1.35f;

    [Header("Left/Right Direction - Jump Obstacles")]
    [SerializeField] private float sideMinSpawnSeconds = 1.0f;
    [SerializeField] private float sideMaxSpawnSeconds = 1.8f;
    [SerializeField] private Vector3 sideObstacleScale = new Vector3(1.1f, 1.1f, 1f);
    [SerializeField] private float sideObstacleLateralRandomness = 0f;

    private float sideSpawnTimer;
    private float nextSideSpawnTime;

    private float nextJetGateDistance;
    private TravelDirection lastDirection;

    private void Start()
    {
        if (GameFlow.Instance != null)
        {
            lastDirection = GameFlow.Instance.CurrentDirection;
            nextJetGateDistance =
                GameFlow.Instance.DistanceTravelled + firstJetGateAfterDistance;
        }

        PickNextSideSpawnTime();
    }

    private void Update()
    {
        if (GameFlow.Instance == null)
            return;

        if (!GameFlow.Instance.IsRunning || GameFlow.Instance.IsGameOver)
            return;

        if (GameFlow.Instance.CurrentDirection != lastDirection)
        {
            OnDirectionChanged();
        }

        if (GameFlow.Instance.CurrentDirection == TravelDirection.Up)
        {
            UpdateJetGateSpawning();
        }
        else
        {
            UpdateSideObstacleSpawning();
        }
    }

    private void OnDirectionChanged()
    {
        lastDirection = GameFlow.Instance.CurrentDirection;

        sideSpawnTimer = 0f;
        PickNextSideSpawnTime();

        if (GameFlow.Instance.CurrentDirection == TravelDirection.Up)
        {
            nextJetGateDistance =
                GameFlow.Instance.DistanceTravelled + firstJetGateAfterDistance;
        }
    }

    private void UpdateJetGateSpawning()
    {
        if (GameFlow.Instance.ScrollSpeedMultiplier <= 0f)
            return;

        if (GameFlow.Instance.DistanceTravelled < nextJetGateDistance)
            return;

        SpawnMovingJetGate();

        nextJetGateDistance = 
            GameFlow.Instance.DistanceTravelled + jetDistanceBetweenGates;
    }

    private void UpdateSideObstacleSpawning()
    {
        sideSpawnTimer += Time.deltaTime;

        if (sideSpawnTimer < nextSideSpawnTime)
            return;

        SpawnSideJumpObstacle();

        sideSpawnTimer = 0f;
        PickNextSideSpawnTime();
    }

    private void SpawnMovingJetGate()
    {
        Vector2 movementDirection = Vector2.up;

        float gapCenterY = Random.Range(
            jetGapCenterMinY,
            jetGapCenterMaxY
        );

        float halfGap = jetGapSize * 0.5f;
        float halfBarHeight = jetBarScale.y * 0.5f;

        GameObject gateRoot = new GameObject("Moving Jet Gate");
        gateRoot.transform.position = movementDirection * spawnDistance;

        ScrollingObject scrollingObject = gateRoot.AddComponent<ScrollingObject>();
        scrollingObject.SetMovementDirection(movementDirection);

        Vector3 topLocalPosition = new Vector3(
            0f,
            gapCenterY + halfGap + halfBarHeight,
            0f
        );

        Vector3 bottomLocalPosition = new Vector3(
            0f,
            gapCenterY - halfGap - halfBarHeight,
            0f
        );

        GameObject topBar = CreateObstacleChild(
            "Top Moving Bar",
            gateRoot.transform,
            topLocalPosition,
            jetBarScale
        );

        GameObject bottomBar = CreateObstacleChild(
            "Bottom Moving Bar",
            gateRoot.transform,
            bottomLocalPosition,
            jetBarScale
        );

        MovingJetGate movingGate = gateRoot.AddComponent<MovingJetGate>();

        movingGate.Initialize(
            topBar.transform,
            bottomBar.transform,
            jetHorizontalRange,
            jetHorizontalSpeed
        );
    }

    private GameObject CreateObstacleChild(
        string objectName,
        Transform parent,
        Vector3 localPosition,
        Vector3 localScale
    )
    {
        GameObject obstacle = Instantiate(obstaclePrefab, parent);

        obstacle.name = objectName;
        obstacle.transform.localPosition = localPosition;
        obstacle.transform.localRotation = Quaternion.identity;
        obstacle.transform.localScale = localScale;

        Collider2D collider = obstacle.GetComponent<Collider2D>();

        if (collider != null)
            collider.isTrigger = true;
        else
            Debug.LogWarning($"{objectName} has no collider2D");

        if (obstacle.GetComponent<Obstacle>() == null)
            obstacle.AddComponent<Obstacle>();

        return obstacle;
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

        CreateSideObstacle(spawnPosition, sideObstacleScale, movementDirection);
    }

    private void CreateSideObstacle(Vector3 position, Vector3 scale, Vector2 movementDirection)
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

    private void PickNextSideSpawnTime()
    {
        nextSideSpawnTime = Random.Range(
            sideMinSpawnSeconds,
            sideMaxSpawnSeconds
        );
    }
}
