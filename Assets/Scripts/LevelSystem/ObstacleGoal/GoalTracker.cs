using UnityEngine;
using System.Collections.Generic;

public class GoalTracker : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    private Dictionary<string, int> initialObstacleCounts = new Dictionary<string, int>();
    private bool hasInitialized = false;

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

        // Update UI with initial counts
        UpdateGoals();
    }

    public Dictionary<string, int> CountObstacles()
    {
        Debug.Log("GoalTracker CountObstacles called");

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

        Debug.Log("GoalTracker: Accessing Storage");
        List<Vector2Int> positions = gridManager.Storage.GetAllPositions();
        Debug.Log($"GoalTracker: Got {positions.Count} positions");

        foreach (Vector2Int pos in positions)
        {
            string objectType = gridManager.Storage.GetTypeAt(pos);
            Debug.Log($"Position {pos}, Type: {objectType}");

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

    // Call this when obstacles are destroyed
    public void UpdateGoals()
    {
        Debug.Log("GoalTracker UpdateGoals called");
        Dictionary<string, int> currentCounts = CountObstacles();

        // Check if all goals are completed
        bool allGoalsComplete = true;
        foreach (var kvp in initialObstacleCounts)
        {
            if (kvp.Value > 0 && currentCounts[kvp.Key] > 0)
            {
                allGoalsComplete = false;
                break;
            }
        }

        if (allGoalsComplete)
        {
            Debug.Log("All goals completed!");
            // Call level complete method here
            // LevelComplete();
        }

        // Notify UI to update
        GoalDisplayer displayer = FindFirstObjectByType<GoalDisplayer>();
        if (displayer != null)
        {
            displayer.DisplayGoals(currentCounts);
        }
    }

    // Call this when an obstacle is destroyed
    public void ObstacleDestroyed(string obstacleType)
    {
        if (obstacleType == "v" || obstacleType == "s" || obstacleType == "bo")
        {
            Debug.Log($"Obstacle destroyed: {obstacleType}");
            UpdateGoals();
        }
    }
}