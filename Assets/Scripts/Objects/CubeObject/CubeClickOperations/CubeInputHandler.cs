using UnityEngine;
using System.Collections.Generic;

public class CubeInputHandler : MonoBehaviour
{
    private GridManager gridManager;
    private GridGroups gridGroups;
    [SerializeField] private CubeFallingHandler fallingHandler;
    [SerializeField] private RocketCreator rocketCreator;
    [SerializeField] private LevelMoveKeeper moveKeeper;

    void Start()
    {

        gridManager = Object.FindFirstObjectByType<GridManager>();
        gridGroups = new GridGroups(gridManager.Storage, gridManager.gridWidth, gridManager.gridHeight);

        // If not assigned in Inspector, try to find it
        if (fallingHandler == null)
        {
            fallingHandler = Object.FindFirstObjectByType<CubeFallingHandler>();
            if (fallingHandler == null)
            {
                Debug.LogError("CubeFallingHandler not found!");
            }
        }
    }

    public void OnCubeClicked(CubeObject cubeObject)
    {
        if (GridEvents.IsProcessing) return;
        Vector2Int? gridPos = FindGridPosition(cubeObject);
        if (gridPos.HasValue)
        {
            List<Vector2Int> group = gridGroups.GetGroup(gridPos.Value);
            if (gridGroups.IsValidGroup(group))
            {
            moveKeeper.movesLeft = moveKeeper.movesLeft - 1 ;
                bool shouldCreateRocket = group.Count >= 4;
                Vector2Int clickedPosition = gridPos.Value;

                // Remove all cubes in the group
                foreach (Vector2Int pos in group)
                {
                    MonoBehaviour mb = gridManager.Storage.GetObjectAt(pos) as MonoBehaviour;
                    if (mb != null)
                    {
                        gridManager.Storage.RemoveObject(pos);
                        Destroy(mb.gameObject);
                    }
                }

                // Create rocket if group size is 4+
                if (shouldCreateRocket)
                {
                    rocketCreator.CreateRocket(clickedPosition);
                }

                if (fallingHandler != null)
                {
                    fallingHandler.ProcessFalling();
                }
            }
        }
        GridEvents.TriggerGridChanged();

    }

    private Vector2Int? FindGridPosition(CubeObject cubeObject)
    {
        List<Vector2Int> allPositions = gridManager.Storage.GetAllPositions();

        foreach (Vector2Int pos in allPositions)
        {
            IGridObject gridObject = gridManager.Storage.GetObjectAt(pos);

            // Cast IGridObject to MonoBehaviour to access gameObject, then check if it has the target component
            MonoBehaviour mb = gridObject as MonoBehaviour;
            if (mb != null && mb.GetComponent<CubeObject>() == cubeObject)
            {
                return pos;
            }
        }

        return null;
    }
}