using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GoalTracker : MonoBehaviour
{
    // Goal counters
    private int vaseCount;
    private int boxCount;
    private int stoneCount;

    // Current remaining obstacles
    private int vaseRemaining;
    private int boxRemaining;
    private int stoneRemaining;

    // UI references
    [SerializeField] private GameObject boxGoalPrefab;
    [SerializeField] private GameObject vaseGoalPrefab;
    [SerializeField] private GameObject stoneGoalPrefab;
    [SerializeField] private Transform goalContainer;

    // References to the text components that show the counts
    private TextMeshProUGUI boxCountText;
    private TextMeshProUGUI vaseCountText;
    private TextMeshProUGUI stoneCountText;

    // Reference to the grid manager
    private GridManager gridManager;

    void Start()
    {
        // Find the grid manager
        gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GoalTracker: Could not find GridManager!");
            return;
        }

        // Initialize goal counts from level data
        InitializeGoalCounts();

        // Subscribe to obstacle destruction events
        SubscribeToEvents();
    }

    private void InitializeGoalCounts()
    {
        LevelData levelData = LevelLoader.Instance.GetLevel(1);
        if (levelData == null)
        {
            Debug.LogError("GoalTracker: Level data is NULL.");
            return;
        }

        // Count obstacles in the level grid
        string[] grid = levelData.grid;
        vaseCount = 0;
        boxCount = 0;
        stoneCount = 0;

        for (int i = 0; i < grid.Length; i++)
        {
            switch (grid[i])
            {
                case "bo": boxCount++; break;
                case "v": vaseCount++; break;
                case "s": stoneCount++; break;
            }
        }

        // Set initial remaining counts
        vaseRemaining = vaseCount;
        boxRemaining = boxCount;
        stoneRemaining = stoneCount;

        // Create UI for each goal type if we have at least one
        // Calculate the layout of the goal indicators
        int totalGoals = 0;
        if (boxCount > 0) totalGoals++;
        if (vaseCount > 0) totalGoals++;
        if (stoneCount > 0) totalGoals++;

        // Setup the layout - position them side by side
        float spacing = 80f; // Space between indicators
        float startX = -(spacing * (totalGoals-1))/2f; // Center the indicators
        int currentIndex = 0;

        RectTransform containerRect = goalContainer as RectTransform;

        if (boxCount > 0)
        {
            GameObject boxGoal = Instantiate(boxGoalPrefab, goalContainer);
            RectTransform rectTransform = boxGoal.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(startX + (currentIndex * spacing), 0);

            boxCountText = boxGoal.GetComponentInChildren<TextMeshProUGUI>();
            if (boxCountText != null) {
                boxCountText.text = boxRemaining.ToString();
                boxCountText.color = Color.black; // Set text color to black for better visibility
                boxCountText.fontSize = 24; // Increase text size
            } else {
                Debug.LogError("TextMeshProUGUI component not found in boxGoal prefab");
            }
            currentIndex++;
        }

        if (vaseCount > 0)
        {
            GameObject vaseGoal = Instantiate(vaseGoalPrefab, goalContainer);
            RectTransform rectTransform = vaseGoal.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(startX + (currentIndex * spacing), 0);

            vaseCountText = vaseGoal.GetComponentInChildren<TextMeshProUGUI>();
            if (vaseCountText != null) {
                vaseCountText.text = vaseRemaining.ToString();
                vaseCountText.color = Color.black; // Set text color to black for better visibility
                vaseCountText.fontSize = 24; // Increase text size
            } else {
                Debug.LogError("TextMeshProUGUI component not found in vaseGoal prefab");
            }
            currentIndex++;
        }

        if (stoneCount > 0)
        {
            GameObject stoneGoal = Instantiate(stoneGoalPrefab, goalContainer);
            RectTransform rectTransform = stoneGoal.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(startX + (currentIndex * spacing), 0);

            stoneCountText = stoneGoal.GetComponentInChildren<TextMeshProUGUI>();
            if (stoneCountText != null) {
                stoneCountText.text = stoneRemaining.ToString();
                stoneCountText.color = Color.black; // Set text color to black for better visibility
                stoneCountText.fontSize = 24; // Increase text size
            } else {
                Debug.LogError("TextMeshProUGUI component not found in stoneGoal prefab");
            }
            currentIndex++;
        }
    }

    private void SubscribeToEvents()
    {
        // Find damage applicator to track obstacle destruction
        DamageApplicator damageApplicator = FindFirstObjectByType<DamageApplicator>();
        if (damageApplicator == null)
        {
            Debug.LogError("GoalTracker: Could not find DamageApplicator!");
            return;
        }

        // Add listener to the grid to track obstacle removal
        // For simplicity, we'll use a polling approach in Update instead
    }

    void Update()
    {
        // Check grid periodically to update goal status
        UpdateGoalStatus();

        // Check if all goals are completed
        CheckLevelCompletion();
    }

    private void UpdateGoalStatus()
    {
        // Count remaining obstacles by type
        int currentVase = 0;
        int currentBox = 0;
        int currentStone = 0;

        // Get all grid positions and check for obstacles
        List<Vector2Int> allPositions = gridManager.Storage.GetAllPositions();
        foreach (Vector2Int pos in allPositions)
        {
            string objectType = gridManager.Storage.GetTypeAt(pos);

            if (objectType == "v") currentVase++;
            else if (objectType == "bo") currentBox++;
            else if (objectType == "s") currentStone++;
        }

        // Update remaining counts
        vaseRemaining = currentVase;
        boxRemaining = currentBox;
        stoneRemaining = currentStone;

        // Update UI if needed
        if (vaseCountText != null && vaseCount > 0)
        {
            vaseCountText.text = vaseRemaining.ToString();
        }

        if (boxCountText != null && boxCount > 0)
        {
            boxCountText.text = boxRemaining.ToString();
        }

        if (stoneCountText != null && stoneCount > 0)
        {
            stoneCountText.text = stoneRemaining.ToString();
        }
    }

    private void CheckLevelCompletion()
    {
        // If all obstacle counts are zero, level is complete
        if (vaseRemaining == 0 && boxRemaining == 0 && stoneRemaining == 0)
        {
            // Find the level manager to notify level completion
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            if (levelManager != null)
            {
                // Call level complete method
                levelManager.OnLevelComplete();
            }
            else
            {
                Debug.LogWarning("GoalTracker: Could not find LevelManager to notify level completion!");

                // As a fallback, load main scene directly
                Debug.Log("Level Complete! All obstacles cleared.");

                // Prevent this from triggering repeatedly
                this.enabled = false;
            }
        }
    }

    // Public method to get if level is complete
    public bool IsLevelComplete()
    {
        return vaseRemaining == 0 && boxRemaining == 0 && stoneRemaining == 0;
    }

    // Public methods to get remaining counts for other components
    public int GetRemainingVases() => vaseRemaining;
    public int GetRemainingBoxes() => boxRemaining;
    public int GetRemainingStones() => stoneRemaining;
    public int GetTotalRemainingObstacles() => vaseRemaining + boxRemaining + stoneRemaining;
}