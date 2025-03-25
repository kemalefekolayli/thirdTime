using UnityEngine;

public class GoalCalculator : MonoBehaviour {

    // Goal counters
    private int vaseCount;
    private int boxCount;
    private int stoneCount;

    // Current remaining obstacles
    private int vaseRemaining;
    private int boxRemaining;
    private int stoneRemaining;

    private void InitializeGoalCounts()
    {
        LevelData levelData = LevelLoader.Instance.GetLevel(1);
        if (levelData == null)
        {
            Debug.LogError("GoalTracker: Level data is NULL.");
            return;
        }


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


        vaseRemaining = vaseCount;
        boxRemaining = boxCount;
        stoneRemaining = stoneCount;
    }

}