using UnityEngine;


public class LevelProgressManager : MonoBehaviour
{

    public static LevelProgressManager Instance { get; private set; }

    // Keys for PlayerPrefs
    private const string CurrentLevelKey = "CurrentLevel";
    private const int DefaultStartLevel = 1;
    private const int MaxLevel = 10; // Assuming 10 levels total as per requirements


    private int currentLevel;

    public int CurrentLevel
    {
        get { return currentLevel; }
    }


    public bool AllLevelsCompleted
    {
        get { return currentLevel > MaxLevel; }
    }

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCurrentLevel();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void LoadCurrentLevel()
    {
        currentLevel = PlayerPrefs.GetInt(CurrentLevelKey, DefaultStartLevel);
        Debug.Log($"Loaded current level: {currentLevel}");
    }

    public void SaveCurrentLevel()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, currentLevel);
        PlayerPrefs.Save();
        Debug.Log($"Saved current level: {currentLevel}");
    }


    public void SetCurrentLevel(int levelNumber)
    {
        if (levelNumber < 1)
        {
            Debug.LogWarning("Cannot set level below 1. Setting to level 1.");
            levelNumber = 1;
        }

        currentLevel = levelNumber;
        SaveCurrentLevel();
        Debug.Log($"Set current level to: {currentLevel}");
    }


    public void AdvanceToNextLevel()
    {
        currentLevel++;
        SaveCurrentLevel();
        Debug.Log($"Advanced to level: {currentLevel}");
    }


    public void ResetProgress()
    {
        currentLevel = DefaultStartLevel;
        SaveCurrentLevel();
        Debug.Log("Progress reset to level 1");
    }


    public string GetLevelButtonText()
    {
        if (AllLevelsCompleted)
        {
            return "Finished";
        }
        else
        {
            return $"Level {currentLevel}";
        }
    }
}