using UnityEngine;

/// <summary>
/// Initializes a level with the correct level data based on the player's progress
/// This script should be placed in the LevelScene
/// </summary>
public class LevelInitializer : MonoBehaviour
{
    [SerializeField] private LevelMoveKeeper levelMoveKeeper;
    [SerializeField] private GridManager gridManager;

    void Start()
    {
        // Find required components if not assigned
        if (levelMoveKeeper == null)
        {
            levelMoveKeeper = FindFirstObjectByType<LevelMoveKeeper>();
        }

        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
        }

        // Load level data
        int currentLevel = GetCurrentLevel();
        if (currentLevel <= 0 || currentLevel > 10)
        {
            Debug.LogError($"Invalid level number: {currentLevel}, defaulting to level 1");
            currentLevel = 1;
        }

        // Initialize the level
        InitializeLevel(currentLevel);
    }

    private int GetCurrentLevel()
    {
        // Try to get level from LevelProgressManager first
        if (LevelProgressManager.Instance != null)
        {
            return LevelProgressManager.Instance.CurrentLevel;
        }

        // Fallback to PlayerPrefs
        return PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    private void InitializeLevel(int levelNumber)
    {
        Debug.Log($"Initializing level {levelNumber}");

        // The level loading is already implemented in GridManager and LevelMoveKeeper
        // Those classes load from LevelLoader.Instance.GetLevel(1)
        // We need to modify them to use the current level number

        // For now, we'll just log a message - you'll need to update the other classes
        // to use the current level number instead of hard-coding level 1
    }
}