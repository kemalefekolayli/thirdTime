using UnityEngine;
using System;

public class ObstacleTracker : MonoBehaviour
{
    private ObstacleCounter obstacleCounter;

    // Event for when an obstacle is destroyed
    public event Action<string> OnObstacleDestroyed;

    // Event for when obstacle counts change
    public event Action OnObstacleCountsChanged;

    void Start()
    {
        obstacleCounter = FindFirstObjectByType<ObstacleCounter>();
        if (obstacleCounter == null)
        {
            Debug.LogError("ObstacleTracker: ObstacleCounter not found!");
        }
    }

    public void TrackObstacleDestruction(string obstacleType)
    {
        if (obstacleCounter == null) return;

        ObstacleCounter.ObstacleCounts counts = obstacleCounter.Counts;

        // Update the appropriate counter
        switch (obstacleType)
        {
            case "bo":
                counts.remainingBoxCount = Mathf.Max(0, counts.remainingBoxCount - 1);
                break;
            case "v":
                counts.remainingVaseCount = Mathf.Max(0, counts.remainingVaseCount - 1);
                break;
            case "s":
                counts.remainingStoneCount = Mathf.Max(0, counts.remainingStoneCount - 1);
                break;
            default:
                Debug.LogWarning($"ObstacleTracker: Unknown obstacle type '{obstacleType}'");
                return;
        }

        // Trigger events
        OnObstacleDestroyed?.Invoke(obstacleType);
        OnObstacleCountsChanged?.Invoke();

        Debug.Log($"Obstacle destroyed: {obstacleType}. Remaining - Boxes: {counts.remainingBoxCount}, Vases: {counts.remainingVaseCount}, Stones: {counts.remainingStoneCount}");
    }
}