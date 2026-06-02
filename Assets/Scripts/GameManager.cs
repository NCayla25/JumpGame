using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MovementDirection CurrentMode = MovementDirection.Vertical;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMode(MovementDirection mode)
    {
        CurrentMode = mode;
    }
}
