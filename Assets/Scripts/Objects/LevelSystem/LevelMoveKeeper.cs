using UnityEngine;

public class LevelMoveKeeper : MonoBehaviour
{
    public int movesLeft;
    public int maxMoves;
    [SerializeField] private FailPopupButton failPopup;
    [SerializeField] private GoalEvaluator goalEvaluator; // Optional

    private bool hasShownFailPopup = false;

    void Start()
    {
        LevelData level1 = LevelLoader.Instance.GetLevel(1);

        if (level1 == null)
        {
            Debug.LogError("Level data is NULL.");
            return;
        }
        maxMoves = level1.move_count;
        movesLeft = level1.move_count;

        // Find references if not set
        if (failPopup == null)
            failPopup = FindFirstObjectByType<FailPopupButton>();

        if (goalEvaluator == null)
            goalEvaluator = FindFirstObjectByType<GoalEvaluator>();
    }

    void Update()
    {
        // Check if moves have run out and we haven't shown the popup yet
        if (movesLeft <= 0 && !hasShownFailPopup)
        {
            // Don't show fail popup if level is already completed
            if (goalEvaluator != null && goalEvaluator.AreAllGoalsCleared())
                return;

            hasShownFailPopup = true;
            ShowFailPopup();
        }
    }

    public bool CanMakeMoves()
    {
        return movesLeft > 0;
    }

    private void ShowFailPopup()
    {
        Debug.Log("Showing fail popup - no moves left!");

        if (failPopup != null)
        {
            failPopup.Show();
        }
        else
        {
            Debug.LogError("Fail popup not found!");
        }
    }
}