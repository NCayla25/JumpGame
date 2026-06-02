using UnityEngine;

public class DirectionButtons : MonoBehaviour
{
    public PlayerController player;

    public void GoLeft()
    {
        player.SetMode(MovementDirection.HorizontalLeft);
    }

    public void GoUp()
    {
        player.SetMode(MovementDirection.Vertical);
    }

    public void GoRight()
    {
        player.SetMode(MovementDirection.HorizontalRight);
    }
}
