using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelCompletionHandler : MonoBehaviour
{
    [SerializeField] private GameObject victoryEffectPrefab;
    [SerializeField] private GameObject failPopupPrefab;
    [SerializeField] private float victoryDelayBeforeMainMenu = 2.0f;

    private GoalEvaluator goalEvaluator;
    private int currentLevelNumber;
    private Canvas canvas;

    void Start()
    {
        goalEvaluator = FindFirstObjectByType<GoalEvaluator>();
        canvas = FindFirstObjectByType<Canvas>();

        if (goalEvaluator == null)
            Debug.LogError("LevelCompletionHandler: GoalEvaluator not found!");

        // Load current level
        currentLevelNumber = PlayerPrefs.GetInt("CurrentLevel", 1);

        // Subscribe to events
        if (goalEvaluator != null)
        {
            goalEvaluator.OnGoalsCleared += HandleLevelCompleted;
            goalEvaluator.OnLevelFailed += HandleLevelFailed;
        }
    }

    private void HandleLevelCompleted()
    {
        Debug.Log("Level completed!");

        // Play victory effect if available
        if (victoryEffectPrefab != null)
        {
            Instantiate(victoryEffectPrefab, Vector3.zero, Quaternion.identity);
        }

        // Update player progress
        int nextLevel = currentLevelNumber + 1;
        PlayerPrefs.SetInt("CurrentLevel", nextLevel);
        PlayerPrefs.Save();

        // Wait before transitioning to main menu
        StartCoroutine(ReturnToMainMenuAfterDelay(victoryDelayBeforeMainMenu));
    }

    private void HandleLevelFailed()
    {
        Debug.Log("Level failed!");

        // Show failure popup
        ShowFailPopup();
    }

    private void ShowFailPopup()
    {
        if (failPopupPrefab != null && canvas != null)
        {
            Instantiate(failPopupPrefab, canvas.transform);
        }
        else
        {
            Debug.LogError("Cannot show fail popup: missing prefab or canvas!");
        }
    }

    private IEnumerator ReturnToMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainScene");
    }
}