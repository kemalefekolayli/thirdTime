using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainSceneName = "MainScene";
    [SerializeField] private string levelSceneName = "LevelScene";

    [Header("Transition Settings")]
    [SerializeField] private float transitionDelay = 1.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevelScene()
    {
        SceneManager.LoadScene(levelSceneName);
    }


    public void LoadMainScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public void HandleVictory()
    {
        StartCoroutine(VictorySequence());
    }

    private IEnumerator VictorySequence()
    {
        // Play celebration animation
        CelebrationManager.PlayWinAnimation();

        // Wait for celebration animation
        yield return new WaitForSeconds(transitionDelay);

        // Advance to next level (only if not all levels are completed)
        LevelProgressManager progressManager = LevelProgressManager.Instance;
        if (progressManager != null && !progressManager.AllLevelsCompleted)
        {
            progressManager.AdvanceToNextLevel();
        }

        // Return to main scene
        LoadMainScene();
    }


    public void RestartLevel()
    {
        SceneManager.LoadScene(levelSceneName);
    }
    private void OnSceneUnloaded(Scene scene)
    {
        // Find any singleton components that might have references to scene-specific objects
        GoalTracker goalTracker = FindFirstObjectByType<GoalTracker>();
        if (goalTracker != null)
        {
            // Null out references or unsubscribe from events to prevent errors
            goalTracker.CleanupReferences();
        }
    }
}