using UnityEngine;
using System.Collections.Generic;

public class ObstacleCounter : MonoBehaviour
{
    [System.Serializable]
    public class ObstacleCounts
    {
        public int totalBoxCount;
        public int totalVaseCount;
        public int totalStoneCount;
        public int remainingBoxCount;
        public int remainingVaseCount;
        public int remainingStoneCount;
    }

    private ObstacleCounts counts = new ObstacleCounts();
    private int currentLevelNumber;

    public ObstacleCounts Counts => counts;

    void Start()
    {
        // Load level data from player preferences
        currentLevelNumber = PlayerPrefs.GetInt("CurrentLevel", 1);

        // Initialize goal counts
        InitializeGoalCounts();
    }

    private void InitializeGoalCounts()
    {
        LevelData levelData = LevelLoader.Instance.GetLevel(currentLevelNumber);
        if (levelData == null)
        {
            Debug.LogError("ObstacleCounter: Level data is NULL.");
            return;
        }

        // Count all obstacles in the level data
        string[] grid = levelData.grid;
        counts.totalBoxCount = 0;
        counts.totalVaseCount = 0;
        counts.totalStoneCount = 0;

        for (int i = 0; i < grid.Length; i++)
        {
            switch (grid[i])
            {
                case "bo": counts.totalBoxCount++; break;
                case "v": counts.totalVaseCount++; break;
                case "s": counts.totalStoneCount++; break;
            }
        }

        // Set remaining counts to total counts initially
        counts.remainingBoxCount = counts.totalBoxCount;
        counts.remainingVaseCount = counts.totalVaseCount;
        counts.remainingStoneCount = counts.totalStoneCount;
    }

    // Public method to get total obstacles
    public int GetTotalObstacleCount()
    {
        return counts.totalBoxCount + counts.totalVaseCount + counts.totalStoneCount;
    }

    // Public method to get remaining obstacles
    public int GetRemainingObstacleCount()
    {
        return counts.remainingBoxCount + counts.remainingVaseCount + counts.remainingStoneCount;
    }
}