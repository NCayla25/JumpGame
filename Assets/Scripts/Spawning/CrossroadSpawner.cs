using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class CrossroadSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject crossroadPrefab;

    [Header("Crossroad Timing")]
    [SerializeField] private float firstCrossroadDelay = 20f;
    [SerializeField] private float secondsBetweenCrossroads = 30f;

    [Header("Spawn")]
    [SerializeField] private float spawnDistance = 10f;

    private float timer;
    private bool firstCrossroadSpawned;

    private void Update()
    {
        if (GameFlow.Instance == null)
            return;

        if (!GameFlow.Instance.IsRunning || GameFlow.Instance.IsGameOver)
            return;

        float timerMultiplier = GetTimerMultiplier();

        if (timerMultiplier <= 0f)
            return;
        
        timer += Time.deltaTime * timerMultiplier;

        float targetTime = firstCrossroadSpawned
            ? secondsBetweenCrossroads
            : firstCrossroadDelay;

        if (timer >= targetTime)
        {
            SpawnCrossroad();
            timer = 0f;
            firstCrossroadSpawned = true;
        }
    }

    private float GetTimerMultiplier()
    {
        if (GameFlow.Instance == null)
            return 0f;

        if (GameFlow.Instance.CurrentDirection == TravelDirection.Up)
            return GameFlow.Instance.ScrollSpeedMultiplier;

        return 1f;
    }

    private void SpawnCrossroad()
    {
        if (crossroadPrefab == null)
        {
            Debug.LogWarning("CrossroadSpawner has no crossroad prefab assigned.");
            return;
        }

        Vector2 direction = GameFlow.Instance.DirectionVector;
        Vector3 position = direction * spawnDistance;

        GameObject crossroad = Instantiate(crossroadPrefab, position, Quaternion.identity);

        ScrollingObject scrollingObject = crossroad.GetComponent<ScrollingObject>();

        if (scrollingObject == null)
            scrollingObject = crossroad.AddComponent<ScrollingObject>();

        scrollingObject.SetMovementDirection(direction);

        if (crossroad.GetComponent<CrossroadTrigger>() == null)
            crossroad.AddComponent<CrossroadTrigger>();
    }
}
