using Unity.VisualScripting;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public static GroundSpawner Instance;

    public GameObject groundTilePrefab;

    private Vector2 spawnPosition = Vector2.zero;

    private PlayerController player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        
        for (int i = 0; i < 5; i++)
        {
            SpawnTile();
        }
    }

    public void SpawnTile()
    {
        Instantiate(
            groundTilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        switch (player.currentDirection)
        {
            case PlayerController.Direction.Up:
                spawnPosition += Vector2.up * 5;
                break;

            case PlayerController.Direction.Left:
                spawnPosition += Vector2.left * 5;
                break;

            case PlayerController.Direction.Right:
                spawnPosition += Vector2.right * 5;
                break;
        }
    }
}
