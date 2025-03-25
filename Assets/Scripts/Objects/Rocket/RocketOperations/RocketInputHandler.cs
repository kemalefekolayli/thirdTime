using UnityEngine;
using System.Collections.Generic;

public class RocketInputHandler : MonoBehaviour
{
    private GridManager gridManager;
    [SerializeField] private RocketExplosionManager explosionManager;
    [SerializeField] private CubeFallingHandler fallingHandler;
    [SerializeField] private LevelMoveKeeper moveKeeper;

    void Start()
    {
        gridManager = Object.FindFirstObjectByType<GridManager>();

        if (explosionManager == null)
        {
            explosionManager = Object.FindFirstObjectByType<RocketExplosionManager>();
            if (explosionManager == null)
            {
                Debug.LogError("RocketInputHandler: RocketExplosionManager reference not found!");
            }
        }

        if (fallingHandler == null)
        {
            fallingHandler = Object.FindFirstObjectByType<CubeFallingHandler>();
        }
    }

public void OnRocketClicked(RocketObject rocketObject)
{
    // Store the rocket position for use in the queued action
    Vector2Int? gridPos = FindGridPosition(rocketObject);

    if (gridPos.HasValue)
    {
        // Enqueue the rocket click action
        GameActionQueue.Instance.EnqueueAction(() => {
            ProcessRocketClick(gridPos.Value, rocketObject);
        });
    }
}

private void ProcessRocketClick(Vector2Int gridPos, RocketObject rocketObject)
{
    string rocketType = gridManager.Storage.GetTypeAt(gridPos);

    // Get rocket game object before removing from storage
    MonoBehaviour mb = gridManager.Storage.GetObjectAt(gridPos) as MonoBehaviour;
    if (mb != null)
    {
        GameObject rocketGameObject = mb.gameObject;

        // Remove from grid storage
        gridManager.Storage.RemoveObject(gridPos);

        // Destroy the rocket
        Destroy(rocketGameObject);

        // Trigger explosion
        if (explosionManager != null)
        {
            explosionManager.ExplodeRocket(gridPos, rocketType);
        }
        else
        {
            // Fallback if explosion manager not available
            Debug.LogWarning("RocketInputHandler: No explosion manager, falling back to simple removal");
            if (fallingHandler != null)
            {
                fallingHandler.ProcessFalling();
            }
        }
    }

    moveKeeper.movesLeft = moveKeeper.movesLeft - 1;
}

    private Vector2Int? FindGridPosition(RocketObject rocketObject)
    {
        List<Vector2Int> allPositions = gridManager.Storage.GetAllPositions();

        foreach (Vector2Int pos in allPositions)
        {
            IGridObject gridObject = gridManager.Storage.GetObjectAt(pos);

            // Cast IGridObject to MonoBehaviour to access gameObject, then check if it has the target component
            MonoBehaviour mb = gridObject as MonoBehaviour;
            if (mb != null && mb.GetComponent<RocketObject>() == rocketObject)
            {
                return pos;
            }
        }

        return null;
    }
}