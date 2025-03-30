using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActionQueue : MonoBehaviour
{
    private Queue<Action> actionQueue = new Queue<Action>();
    private bool isProcessing = false;
    public LevelMoveKeeper levelMoveKeeper;
    public GoalTracker goalTracker;

    // Flag to track if level is completed
    public bool isLevelCompleted = false;

    // Flag to track if level is failed
    public bool isLevelFailed = false;

    // Singleton pattern
    public static GameActionQueue Instance { get; private set; }

    private bool shouldDecreaseMoves = true;

    public void SkipMoveDecrease()
    {
        shouldDecreaseMoves = false;
    }

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
        EnsureReferences();
    }

    private void Start()
    {
    EnsureReferences();
        // Subscribe to GoalTracker's event
        if (goalTracker != null)
        {
            goalTracker.OnAllGoalsCompleted += HandleLevelCompleted;
        }
    }

    private void OnDestroy()
    {
        // Clean up subscription
        if (goalTracker != null)
        {
            goalTracker.OnAllGoalsCompleted -= HandleLevelCompleted;
        }
    }

    private void HandleLevelCompleted()
    {
        if (!isLevelCompleted && !isLevelFailed)
        {
            isLevelCompleted = true;
            Debug.Log("LEVEL COMPLETED HANDLER CALLED");
            CelebrationManager.PlayWinAnimation();

            // Clear the queue
            actionQueue.Clear();

            // Stop processing
            StopAllCoroutines();
            isProcessing = false;
        }
    }

    private void HandleLevelFailed()
    {
        if (!isLevelFailed && !isLevelCompleted)
        {
            isLevelFailed = true;
            Debug.Log("LEVEL FAILED - Out of moves!");
            DefeatScreen.Show();

            // Clear the queue
            actionQueue.Clear();

            // Stop processing
            StopAllCoroutines();
            isProcessing = false;
        }
    }

    // Add a new action to the queue
    public void EnqueueAction(Action action)
    {
        EnsureReferences();
        // Don't accept any more actions if level is completed or failed
        if (isLevelCompleted || isLevelFailed)
        {
            Debug.Log("Level is completed or failed, no more actions accepted");
            return;
        }

        Debug.LogError($"Current Moves: {levelMoveKeeper.currentMoves}");

        // Check if we're about to use the last move
        bool isLastMove = levelMoveKeeper.currentMoves <= 1;

        Debug.LogError("enqueued action");
        actionQueue.Enqueue(action);

        // Start processing if not already doing so
        if (!isProcessing)
        {
            StartCoroutine(ProcessQueue());
        }

        if (shouldDecreaseMoves)
        {
            levelMoveKeeper.DecreaseMove();

            // Check if we've just run out of moves after decreasing
            if (levelMoveKeeper.currentMoves <= 0)
            {
                // Wait to see if this last move completes the level before showing defeat
                StartCoroutine(CheckForDefeatAfterLastMove());
            }
        }
        shouldDecreaseMoves = true;
    }

    // Coroutine to check if the level is failed after the last move
    private IEnumerator CheckForDefeatAfterLastMove()
    {
        // Wait until the current processing is done
        yield return new WaitUntil(() => !isProcessing || isLevelCompleted);

        // If level wasn't completed after the last move, it's a defeat
        if (!isLevelCompleted && !isLevelFailed && levelMoveKeeper.currentMoves <= 0)
        {
            HandleLevelFailed();
        }
    }

    // Process the queue one action at a time
    private IEnumerator ProcessQueue()
    {
    EnsureReferences();

    if (levelMoveKeeper == null)
        levelMoveKeeper = FindFirstObjectByType<LevelMoveKeeper>();
    if (goalTracker == null)
        goalTracker = FindFirstObjectByType<GoalTracker>();
        isProcessing = true;

        // Immediately check if goals are already complete
        if (goalTracker.AreAllGoalsCompleted())
        {
            HandleLevelCompleted();
            yield break;
        }

        // Process all actions in the queue
        while (actionQueue.Count > 0 && !isLevelCompleted && !isLevelFailed)
        {
            // Get the next action
            Action nextAction = actionQueue.Dequeue();

            // Execute it
            nextAction.Invoke();

            // Wait a frame to allow any coroutines to start
            yield return null;

            // Wait until all relevant systems are idle before proceeding
            yield return new WaitUntil(() => AllSystemsIdle());

            // Small buffer to ensure stability
            yield return new WaitForSeconds(0.1f);

            // Check if we're still processing (might have been stopped by HandleLevelCompleted)
            if (isLevelCompleted || isLevelFailed)
            {
                break;
            }

            // Update goals after action is completed and all systems are idle
            goalTracker.UpdateGoals();
        }

        // Only set to false if not completed or failed
        if (!isLevelCompleted && !isLevelFailed)
        {
            isProcessing = false;
        }
    }

    // Check if all game systems are idle and ready for the next action
    private bool AllSystemsIdle()
    {
        CubeFallingHandler fallingHandler = FindFirstObjectByType<CubeFallingHandler>();
        GridFiller gridFiller = FindFirstObjectByType<GridFiller>();

        // Check if all systems are idle
        bool systemsIdle =
            (fallingHandler == null || !fallingHandler.IsProcessing) &&
            (gridFiller == null || !gridFiller.IsProcessing);

        return systemsIdle;
    }

    public void ResetState()
    {
        isLevelCompleted = false;
        isLevelFailed = false;
        actionQueue.Clear();
        isProcessing = false;
    }
    private void EnsureReferences()
    {
        if (levelMoveKeeper == null)
            levelMoveKeeper = FindFirstObjectByType<LevelMoveKeeper>();
        if (goalTracker == null)
            goalTracker = FindFirstObjectByType<GoalTracker>();
    }
}