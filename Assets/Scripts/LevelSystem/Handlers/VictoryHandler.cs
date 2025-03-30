using UnityEngine;
using System.Collections;

/// <summary>
/// Responds to level completion and triggers the victory sequence
/// </summary>
public class VictoryHandler : MonoBehaviour
{
    [SerializeField] private float victoryDelay = 1.5f;
    private GameActionQueue gameActionQueue;
    private GoalTracker goalTracker;
    private bool victoryTriggered = false;

    private void Start()
    {
        // Get required components
        gameActionQueue = FindFirstObjectByType<GameActionQueue>();
        goalTracker = FindFirstObjectByType<GoalTracker>();

        if (goalTracker != null)
        {
            // Subscribe to the goal completion event
            goalTracker.OnAllGoalsCompleted += OnLevelCompleted;
        }
        else
        {
            Debug.LogError("VictoryHandler: GoalTracker not found!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed
        if (goalTracker != null)
        {
            goalTracker.OnAllGoalsCompleted -= OnLevelCompleted;
        }
    }

    /// <summary>
    /// Called when all level goals are completed
    /// </summary>
    private void OnLevelCompleted()
    {
        if (!victoryTriggered)
        {
            victoryTriggered = true;
            StartCoroutine(TriggerVictorySequence());
        }
    }

    private IEnumerator TriggerVictorySequence()
    {
        // Show the victory screen if it exists (optional)
        VictoryScreen.Show();

        // Small delay to show celebration effects
        yield return new WaitForSeconds(victoryDelay);

        // Tell the SceneController to handle the victory
        if (SceneController.Instance != null)
        {
            SceneController.Instance.HandleVictory();
        }
        else
        {
            Debug.LogError("VictoryHandler: SceneController instance not found!");
        }
    }
}