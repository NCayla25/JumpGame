using UnityEngine;

public class DirectionManager : MonoBehaviour
{
    public static DirectionManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject choicePanel;

    [Header("Choice Window")]
    [SerializeField] private float choiceWindowSeconds = 1.75f;

    private bool choiceWindowOpen;
    private float choiceTimer;
    private CrossroadTrigger activeCrossroad;

    public bool IsChoiceWindowOpen => choiceWindowOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (PlayerInputReader.Instance != null)
            return;

        PlayerInputReader.Instance.UpPressed += ChooseUp;
        PlayerInputReader.Instance.LeftPressed += ChooseLeft;
        PlayerInputReader.Instance.RightPressed += ChooseRight;
    }

    private void OnDisable()
    {
        if (PlayerInputReader.Instance == null)
            return;

        PlayerInputReader.Instance.UpPressed -= ChooseUp;
        PlayerInputReader.Instance.LeftPressed -= ChooseLeft;
        PlayerInputReader.Instance.RightPressed -= ChooseRight;
    }

    private void Update()
    {
        if (!choiceWindowOpen)
            return;

        choiceTimer += Time.deltaTime;

        if (choiceTimer >= choiceWindowSeconds)
        {
            CloseChoiceWindow();
        }
    }

    public void OpenChoiceWindow(CrossroadTrigger crossroad)
    {
        if (GameFlow.Instance == null || GameFlow.Instance.IsGameOver)
            return;

        if (choiceWindowOpen)
            return;

        activeCrossroad = crossroad;
        choiceTimer = 0f;
        choiceWindowOpen = true;

        if (choicePanel != null)
            choicePanel.SetActive(true);
    }

    public void ChooseUp()
    {
        ChooseDirection(TravelDirection.Up);
    }

    public void ChooseLeft()
    {
        ChooseDirection(TravelDirection.Left);
    }

    public void ChooseRight()
    {
        ChooseDirection(TravelDirection.Right);
    }

    private void ChooseDirection(TravelDirection direction)
    {
        if (!choiceWindowOpen)
            return;

        GameFlow.Instance.SetDirection(direction);

        if (activeCrossroad != null)
            activeCrossroad.MarkUsed();

        CloseChoiceWindow();
    }

    private void CloseChoiceWindow()
    {
        choiceWindowOpen = false;
        choiceTimer = 0f;
        activeCrossroad = null;
        
        if (choicePanel != null)
            choicePanel.SetActive(false);
    }
}
