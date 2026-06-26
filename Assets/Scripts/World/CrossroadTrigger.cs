using UnityEngine;

public class CrossroadTrigger : MonoBehaviour
{
    private bool used;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (DirectionManager.Instance != null)
        {
            DirectionManager.Instance.OpenChoiceWindow(this);
        }
    }

    public void MarkUsed()
    {
        used = true;
    }
}
