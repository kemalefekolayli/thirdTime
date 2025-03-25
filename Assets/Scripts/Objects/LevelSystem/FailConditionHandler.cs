using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FailConditionHandler : MonoBehaviour
{
    [SerializeField] private LevelMoveKeeper moveKeeper;
    [SerializeField] private FailPopupButton failPopup;
    [SerializeField] private GoalEvaluator goalEvaluator; // Optional, to check if level is already won


    private void Start()
    {
        if (moveKeeper == null)
            moveKeeper = FindFirstObjectByType<LevelMoveKeeper>();

        if (failPopup == null)
            failPopup = FindFirstObjectByType<FailPopupButton>();

        if (goalEvaluator == null)
            goalEvaluator = FindFirstObjectByType<GoalEvaluator>();

        // Subscribe to the moves run out event
        if (moveKeeper != null)
            moveKeeper.OnMovesRunOut += HandleMovesRunOut;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (moveKeeper != null)
            moveKeeper.OnMovesRunOut -= HandleMovesRunOut;
    }

    private void HandleMovesRunOut()
{
    // Check if player has already won
    if (goalEvaluator != null && goalEvaluator.AreAllGoalsCleared())
        return; // Don't show fail popup if player actually won

    Debug.Log("No more moves left - showing fail popup");

    // Ensure any ongoing processes complete
    StartCoroutine(ShowFailPopupAfterDelay(0.5f));
}

private IEnumerator ShowFailPopupAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);

    // Show the fail popup
    if (failPopup != null)
        failPopup.Show();
    else
        Debug.LogError("FailPopupController not found!");
}
}