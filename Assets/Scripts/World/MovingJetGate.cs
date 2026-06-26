using UnityEngine;

public class MovingJetGate : MonoBehaviour
{
    [Header("Moving Pieces")]
    [SerializeField] private Transform topBar;
    [SerializeField] private Transform bottomBar;

    [Header("Horizontal Motion")]
    [SerializeField] private float horizontalRange = 2.6f;
    [SerializeField] private float horizontalSpeed = 1.4f;
    [SerializeField] private float startingOffset;

    private Vector3 topStartLocalPosition;
    private Vector3 bottomStartLocalPosition;

    private void Awake()
    {
        if (topBar != null)
            topStartLocalPosition = topBar.localPosition;

        if (bottomBar != null)
            bottomStartLocalPosition = bottomBar.localPosition;

        startingOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float pingPong = Mathf.PingPong(
            (Time.time + startingOffset) * horizontalSpeed,
            horizontalRange * 2f
        );

        float xOffset = pingPong - horizontalRange;

        if (topBar != null)
        {
            topBar.localPosition = topStartLocalPosition + Vector3.right * xOffset;
        }

        if (bottomBar != null)
        {
            bottomBar.localPosition = bottomStartLocalPosition + Vector3.right * xOffset;
        }
    }

    public void Initialize(
        Transform newTopBar,
        Transform newBottomBar,
        float newHorizontalRange,
        float newHorizontalSpeed
        )
    {
        topBar = newTopBar;
        bottomBar = newBottomBar;
        horizontalRange = newHorizontalRange;
        horizontalSpeed = newHorizontalSpeed;

        if (topBar != null)
            topStartLocalPosition = topBar.localPosition;

        if (bottomBar != null)
            bottomStartLocalPosition = bottomBar.localPosition;

        startingOffset = Random.Range(0f, 100f);
    }
}
