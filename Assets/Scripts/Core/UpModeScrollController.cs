using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;

public class UpModeScrollController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Up Mode Scroll Trigger")]
    [SerializeField] private float scrollStartsAboveY = 2f;

    [Header("Scroll Feel")]
    [SerializeField] private float upModeScrollMultiplierWhenActive = 1f;

    [Header("Fallback")]
    [SerializeField] private bool autoFindPlayerByTag = true;
    [SerializeField] private bool stopScrollingIfPlayerMissing = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private bool warnedAboutMissingPlayer;

    public bool IsUpModeScrollAllowed { get; private set; }

    private void Start()
    {
        TryFindPlayer();
    }

    private void Update()
    {
        if (GameFlow.Instance == null)
            return;

        if (GameFlow.Instance.IsGameOver)
        {
            IsUpModeScrollAllowed = true;
            SetMultiplier(0f);
            return;
        }

        if (GameFlow.Instance.CurrentDirection != TravelDirection.Up)
        {
            IsUpModeScrollAllowed = true;
            SetMultiplier(1f);
            return;
        }

        if (player == null)
            TryFindPlayer();

        if (player == null)
        {
            if (!warnedAboutMissingPlayer)
            {
                Debug.LogWarning(
                    "UpModeScrollController could not find the Player." +
                    "Assign the Player Transform manually or make sure the root Player object has the Player tagged."
                    );

                warnedAboutMissingPlayer = true;
            }

            IsUpModeScrollAllowed = !stopScrollingIfPlayerMissing;
            GameFlow.Instance.SetScrollSpeedMultiplier(
                stopScrollingIfPlayerMissing ? 0f : 1f
                );

            return;
        }

        bool playerIsAboveScrollLine = player.position.y > scrollStartsAboveY;

        IsUpModeScrollAllowed = playerIsAboveScrollLine;

        float multiplier = playerIsAboveScrollLine
            ? upModeScrollMultiplierWhenActive
            : 0f;

        GameFlow.Instance.SetScrollSpeedMultiplier(multiplier);

        if (showDebugLogs)
        {
            Debug.Log(
                $"UpModeScrollController | " +
                $"Player Y: {player.position.y:F2} | " +
                $"Scroll Allowed: {IsUpModeScrollAllowed} | " +
                $"Multiplier: {multiplier:F2}"
                );
        }
    }

    private void TryFindPlayer()
    {
        if (!autoFindPlayerByTag)
            return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            warnedAboutMissingPlayer = false;
        }
    }

    private void SetMultiplier(float multiplier)
    {
        GameFlow.Instance.SetScrollSpeedMultiplier(multiplier);

        if (showDebugLogs)
        {
            Debug.Log(
                $"Up Scroll Multiplier: {multiplier:F2}, " +
                $"Player Y: {(player != null ? player.position.y.ToString("F2") : "missing")}"
                );
        }
    }
}
