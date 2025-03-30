using UnityEngine;
using System.Collections.Generic;


public class BoxBlastObserver : MonoBehaviour, IBlastObserver
{
    private BoxObstacle boxObstacle;
    private GridManager gridManager;
    private Vector2Int myPosition;
    private bool isRegistered = false;

    private void Awake()
    {
        boxObstacle = GetComponent<BoxObstacle>();
        if (boxObstacle == null)
        {
            Debug.LogError("BoxBlastObserver must be attached to a GameObject with BoxObstacle!");
            return;
        }
    }

    public void Initialize(Vector2Int pos, GridManager gm)
    {
        myPosition = pos;
        gridManager = gm;

        // Register with the blast notifier
        BlastNotifier.Instance.RegisterObserver(this);
        isRegistered = true;
    }

    private void OnDestroy()
    {
        if (isRegistered)
            BlastNotifier.Instance.UnregisterObserver(this);
    }

    public void OnBlastOccurred(List<Vector2Int> blastGroup, int blastId)
    {
        // Check if any position in the blast group is adjacent to my position
        if (IsAdjacentToBlast(blastGroup))
        {
            // Take damage
            boxObstacle.TakeDamage(DamageType.Adjacent, 1);

            // If destroyed, remove from grid and notify GoalTracker
            if (boxObstacle.IsDestroyed)
            {
                if (gridManager != null && gridManager.Storage != null)
                {
                    // Get type before removing
                    string obstacleType = gridManager.Storage.GetTypeAt(myPosition);

                    // Tell GoalTracker about destroyed obstacle
                    GoalTracker goalTracker = FindFirstObjectByType<GoalTracker>();
                    if (goalTracker != null)
                    {
                        goalTracker.ObstacleDestroyed(obstacleType);
                    }

                    // Remove from grid
                    gridManager.Storage.RemoveObject(myPosition);
                }
            }
        }
    }

    private bool IsAdjacentToBlast(List<Vector2Int> blastGroup)
    {
        Vector2Int[] directions = {
            new Vector2Int(0, 1),  // Up
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1, 0)  // Left
        };

        foreach (Vector2Int blastPos in blastGroup)
        {
            foreach (Vector2Int dir in directions)
            {
                Vector2Int adjacentPos = blastPos + dir;
                if (adjacentPos == myPosition)
                {
                    return true;
                }
            }
        }

        return false;
    }
}