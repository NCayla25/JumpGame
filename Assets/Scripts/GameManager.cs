using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private PlayerController player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    public void PlayerTurnLeft()
    {
        player.TurnLeft();
    }

    public void PlayerTurnRight()
    {
        player.TurnRight();
    }
}
