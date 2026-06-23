using UnityEngine;

public class GroundTile : MonoBehaviour 
{
    public Transform nextSpawnPoint;

    private bool spawnedNext = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (spawnedNext)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            spawnedNext = true;

            GroundSpawner.Instance.SpawnTile();
        }
    }
}
