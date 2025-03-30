using UnityEngine;
using System.Collections.Generic;

public class LevelMoveKeeper : MonoBehaviour {
    public int maxMoves;
    public int currentMoves;

    void Start(){
        // Get current level number from LevelProgressManager
        int currentLevel = 1;
        if (LevelProgressManager.Instance != null) {
            currentLevel = LevelProgressManager.Instance.CurrentLevel;
        } else {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        }

        // Ensure level number is valid (1-10)
        currentLevel = Mathf.Clamp(currentLevel, 1, 10);

        // Load the correct level data
        LevelData levelData = LevelLoader.Instance.GetLevel(currentLevel);
        if (levelData == null)
        {
            Debug.LogError($"Level data for level {currentLevel} is NULL. Falling back to level 1.");
            levelData = LevelLoader.Instance.GetLevel(1);

            if (levelData == null) {
                Debug.LogError("Fallback level data is also NULL.");
                // Set default values to prevent crashes
                maxMoves = 20;
                currentMoves = 20;
                return;
            }
        }

        maxMoves = levelData.move_count;
        currentMoves = levelData.move_count;
    }

    public void DecreaseMove() {
        this.currentMoves--;
        Debug.Log($"Moves remaining: {currentMoves}");
    }
}