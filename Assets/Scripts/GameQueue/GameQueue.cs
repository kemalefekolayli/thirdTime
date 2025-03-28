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
    }

    // Add a new action to the queue
    public void EnqueueAction(Action action)
    {

        Debug.LogError($"Current Moves: {levelMoveKeeper.currentMoves}");
        if(levelMoveKeeper.currentMoves <= 0 ){
        return ; }
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
        }
        shouldDecreaseMoves = true;
    }

    // Process the queue one action at a time
    private IEnumerator ProcessQueue()
    {

        isProcessing = true;


        while (actionQueue.Count > 0)
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

            goalTracker.UpdateGoals();
        }

        isProcessing = false;

    }

    // Check if all game systems are idle and ready for the next action
    private bool AllSystemsIdle()
    {
        CubeFallingHandler fallingHandler = FindFirstObjectByType<CubeFallingHandler>();
        GridFiller gridFiller = FindFirstObjectByType<GridFiller>();

        // Add other systems that need to complete before next action

        // Check if all systems are idle
        bool systemsIdle =
            (fallingHandler == null || !fallingHandler.IsProcessing) &&
            (gridFiller == null || !gridFiller.IsProcessing);

        return systemsIdle;
    }
}