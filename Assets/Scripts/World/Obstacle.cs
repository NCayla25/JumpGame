using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private bool alreadyHit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyHit) 
            return;

        if (!other.CompareTag("Player"))
            return;

        PlayerJumpController player = other.GetComponent<PlayerJumpController>();

        if (GameFlow.Instance == null)
            return;

        if (GameFlow.Instance.CurrentDirection == TravelDirection.Up)
        {
            HitPlayer();
            return;
        }

        if (player != null && player.IsGroundJumping)
        {
            alreadyHit = true;
            Debug.Log("Jumped over obstacle.");
            return;
        }

        HitPlayer();
    }

    private void HitPlayer()
    {
        alreadyHit = true;

        if (GameFlow.Instance != null)
        {
            GameFlow.Instance.GameOver();
        }
    }
}
