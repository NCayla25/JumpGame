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

        timer += Time.deltaTime;

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
