using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoalUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text goalText;
    [SerializeField] private Image vaseIcon;
    [SerializeField] private Image boxIcon;
    [SerializeField] private Image stoneIcon;
    [SerializeField] private TMP_Text vaseCountText;
    [SerializeField] private TMP_Text boxCountText;
    [SerializeField] private TMP_Text stoneCountText;

    private ObstacleCounter obstacleCounter;
    private ObstacleTracker obstacleTracker;

    void Start()
    {
        obstacleCounter = FindFirstObjectByType<ObstacleCounter>();
        obstacleTracker = FindFirstObjectByType<ObstacleTracker>();

        if (obstacleCounter == null)
            Debug.LogError("GoalUIController: ObstacleCounter not found!");

        if (obstacleTracker == null)
            Debug.LogError("GoalUIController: ObstacleTracker not found!");

        // Subscribe to events
        if (obstacleTracker != null)
        {
            obstacleTracker.OnObstacleCountsChanged += UpdateUI;
        }

        // Initial UI update
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (obstacleCounter == null) return;

        ObstacleCounter.ObstacleCounts counts = obstacleCounter.Counts;

        // Update goal counts in UI
        if (vaseCountText != null)
            vaseCountText.text = $"{counts.totalVaseCount - counts.remainingVaseCount}/{counts.totalVaseCount}";

        if (boxCountText != null)
            boxCountText.text = $"{counts.totalBoxCount - counts.remainingBoxCount}/{counts.totalBoxCount}";

        if (stoneCountText != null)
            stoneCountText.text = $"{counts.totalStoneCount - counts.remainingStoneCount}/{counts.totalStoneCount}";

        // Hide goal icons if that obstacle type isn't in this level
        if (vaseIcon != null) vaseIcon.gameObject.SetActive(counts.totalVaseCount > 0);
        if (boxIcon != null) boxIcon.gameObject.SetActive(counts.totalBoxCount > 0);
        if (stoneIcon != null) stoneIcon.gameObject.SetActive(counts.totalStoneCount > 0);

        // Update overall goal text
        if (goalText != null)
        {
            int totalRemaining = counts.remainingBoxCount + counts.remainingVaseCount + counts.remainingStoneCount;
            int totalGoals = counts.totalBoxCount + counts.totalVaseCount + counts.totalStoneCount;
            goalText.text = $"Goals: {totalGoals - totalRemaining}/{totalGoals}";
        }
    }
}