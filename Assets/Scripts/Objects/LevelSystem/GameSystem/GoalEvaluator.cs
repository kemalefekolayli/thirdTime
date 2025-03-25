using UnityEngine;
using System;

public class GoalEvaluator : MonoBehaviour
{
    private ObstacleCounter obstacleCounter;
    private ObstacleTracker obstacleTracker;
    private LevelMoveKeeper moveKeeper;

    // Events
    public event Action OnGoalsCleared;
    public event Action OnLevelFailed;

    private bool goalsClearedFired = false;
    private bool levelFailedFired = false;

    void Start()
    {
        obstacleCounter = FindFirstObjectByType<ObstacleCounter>();
        obstacleTracker = FindFirstObjectByType<ObstacleTracker>();
        moveKeeper = FindFirstObjectByType<LevelMoveKeeper>();

        if (obstacleCounter == null)
            Debug.LogError("GoalEvaluator: ObstacleCounter not found!");

        if (obstacleTracker == null)
            Debug.LogError("GoalEvaluator: ObstacleTracker not found!");

        if (moveKeeper == null)
            Debug.LogError("GoalEvaluator: LevelMoveKeeper not found!");

        // Subscribe to obstacle tracker events
        if (obstacleTracker != null)
        {
            obstacleTracker.OnObstacleCountsChanged += EvaluateGoals;
        }
    }

    void Update()
    {
        // Check for failure condition
        if (!goalsClearedFired && !levelFailedFired &&
            moveKeeper != null && moveKeeper.movesLeft <= 0 &&
            !AreAllGoalsCleared())
        {
            levelFailedFired = true;
            OnLevelFailed?.Invoke();
        }
    }

    private void EvaluateGoals()
    {
        if (goalsClearedFired) return;

        if (AreAllGoalsCleared())
        {
            goalsClearedFired = true;
            OnGoalsCleared?.Invoke();
        }
    }

    public bool AreAllGoalsCleared()
    {
        if (obstacleCounter == null) return false;

        ObstacleCounter.ObstacleCounts counts = obstacleCounter.Counts;
        return counts.remainingBoxCount <= 0 &&
               counts.remainingVaseCount <= 0 &&
               counts.remainingStoneCount <= 0;
    }
}