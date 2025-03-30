using UnityEngine;
using System.Collections.Generic;


public class VaseBlastObserver : MonoBehaviour, IBlastObserver
{
    private VaseObstacle vaseObstacle;
    private GridManager gridManager;
    private Vector2Int myPosition;
    private int lastBlastId = -1;
    private bool isRegistered = false;

    private void Awake()
    {
        vaseObstacle = GetComponent<VaseObstacle>();
        if (vaseObstacle == null)
        {
            Debug.LogError("VaseBlastObserver must be attached to a GameObject with VaseObstacle!");
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
        // Skip if we've already processed this blast
        if (blastId == lastBlastId) return;

        // Check if any position in the blast group is adjacent to my position
        if (IsAdjacentToBlast(blastGroup))
        {
            // Remember this blast ID to ensure only one damage per blast
            lastBlastId = blastId;

            // Take damage
            vaseObstacle.TakeDamage(DamageType.Adjacent, 1);

            // If destroyed, remove from grid and notify GoalTracker
            if (vaseObstacle.IsDestroyed)
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
