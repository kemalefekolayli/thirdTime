using UnityEngine;
using System.Collections.Generic;
using System;

public class GoalTracker : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    private Dictionary<string, int> initialObstacleCounts = new Dictionary<string, int>();
    private Dictionary<string, int> currentObstacleCounts = new Dictionary<string, int>();
    private bool hasInitialized = false;

    // Event that other systems can subscribe to
    public event Action OnAllGoalsCompleted;

    void Start()
    {
        Debug.Log("GoalTracker Start called");

        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                Debug.LogError("GoalTracker: Could not find GridManager!");
                return;
            }
        }

        // Wait for GridManager to fully initialize
        Invoke("DelayedInitialization", 0.5f);
    }

    private void DelayedInitialization()
    {
        Debug.Log("GoalTracker DelayedInitialization called");
        if (hasInitialized) return;
        hasInitialized = true;

        // Store initial counts when level starts
        initialObstacleCounts = CountObstacles();

        // Create a copy for tracking current counts
        currentObstacleCounts = new Dictionary<string, int>(initialObstacleCounts);

        // Update UI with initial counts
        UpdateUI();
    }

    public Dictionary<string, int> CountObstacles()
    {
        Dictionary<string, int> obstacleCounts = new Dictionary<string, int>
        {
            { "v", 0 },  // Vase
            { "s", 0 },  // Stone
            { "bo", 0 }  // Box
        };

        if (gridManager == null || gridManager.Storage == null)
        {
            Debug.LogWarning("GoalTracker: GridManager or Storage is null!");
            return obstacleCounts;
        }

        List<Vector2Int> positions = gridManager.Storage.GetAllPositions();
        Debug.Log($"GoalTracker: Counting {positions.Count} positions");

        foreach (Vector2Int pos in positions)
        {
            string objectType = gridManager.Storage.GetTypeAt(pos);

            if (objectType == "v" || objectType == "s" || objectType == "bo")
            {
                if (obstacleCounts.ContainsKey(objectType))
                {
                    obstacleCounts[objectType]++;
                }
            }
        }

        Debug.Log($"Counted obstacles: Vase={obstacleCounts["v"]}, Stone={obstacleCounts["s"]}, Box={obstacleCounts["bo"]}");
        return obstacleCounts;
    }

    // This method should be called by the GameActionQueue after each move is processed
    public void UpdateGoals()
    {
        // Complete re-count of obstacles to ensure accuracy
        Dictionary<string, int> newCounts = CountObstacles();

        // Update our current counts
        currentObstacleCounts = newCounts;

        // Check if all goals are completed
        CheckGoalCompletion();

        // Update UI
        UpdateUI();
    }

    // Updated to use current counts
    public bool AreAllGoalsCompleted()
    {
        foreach (var kvp in initialObstacleCounts)
        {
            if (kvp.Value > 0 && currentObstacleCounts[kvp.Key] > 0)
            {
                return false;
            }
        }
        return true;
    }

    // Check if goals are completed and fire event if needed
    private void CheckGoalCompletion()
    {
        if (AreAllGoalsCompleted())
        {
            Debug.Log("All goals completed!");
            // Notify subscribers
            OnAllGoalsCompleted?.Invoke();
        }
    }

    // Call this when a specific obstacle is destroyed
    public void ObstacleDestroyed(string obstacleType)
    {
        if (obstacleType == "v" || obstacleType == "s" || obstacleType == "bo")
        {
            Debug.Log($"Obstacle destroyed: {obstacleType}");

            // Decrement the count for this obstacle type
            if (currentObstacleCounts.ContainsKey(obstacleType) && currentObstacleCounts[obstacleType] > 0)
            {
                currentObstacleCounts[obstacleType]--;
                Debug.Log($"Updated {obstacleType} count: {currentObstacleCounts[obstacleType]}");

                // Check if goals are completed
                CheckGoalCompletion();

                // Update the UI
                UpdateUI();
            }
        }
    }

    // Update UI elements with current counts
    private void UpdateUI()
    {
        GoalDisplayer displayer = FindFirstObjectByType<GoalDisplayer>();
        if (displayer != null)
        {
            displayer.DisplayGoals(currentObstacleCounts);
        }
    }
}