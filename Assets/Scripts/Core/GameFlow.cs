using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{
    public static GameFlow Instance { get; private set; }

    public event Action<TravelDirection, TravelDirection> DirectionChanged;

    [Header("Movement")]
    [SerializeField] private TravelDirection startingDirection = TravelDirection.Up;
    [SerializeField] private float scrollSpeed = 5f;

    [Header("State")]
    [SerializeField] private bool startRunning = true;

    public TravelDirection CurrentDirection { get; private set; }
    public Vector2 DirectionVector => CurrentDirection.ToVector2();
    public float ScrollSpeed => scrollSpeed;
    public bool IsGameOver { get; private set; }
    public bool IsRunning { get; private set; }

    public float DistanceTravelled { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentDirection = startingDirection;
        IsRunning = startRunning;
    }

    private void Update()
    {
        if (!IsRunning || IsGameOver)
        {
            return;
        }

        DistanceTravelled += scrollSpeed * Time.deltaTime;
    }

    public void SetDirection(TravelDirection newDirection)
    {
        if (IsGameOver)
            return;

        TravelDirection oldDirection = CurrentDirection;
        CurrentDirection = newDirection;

        if (oldDirection != newDirection)
        {
            DirectionChanged?.Invoke(oldDirection, newDirection);
        }
    }

    public void GameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        IsRunning = false;

        Debug.Log("Game Over");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
