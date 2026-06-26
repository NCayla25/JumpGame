using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (GameFlow.Instance == null)
            return;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(GameFlow.Instance.IsGameOver);
    }
}
