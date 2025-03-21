using UnityEngine;
using System.Collections.Generic;

public class CubeInputHandler : MonoBehaviour
{
    private GridManager gridManager;
    private GridGroups gridGroups;
    [SerializeField] private CubeFallingHandler fallingHandler;

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
        Debug.Log("OnCubeClicked called with: " + cubeObject.name);

        // Find the grid position of the clicked cube
        Vector2Int? gridPos = FindGridPosition(cubeObject);

        Debug.Log("Found grid position: " + (gridPos.HasValue ? gridPos.Value.ToString() : "null"));

        if (gridPos.HasValue)
        {
            string cubeType = gridManager.Storage.GetTypeAt(gridPos.Value);
            Debug.Log("Cube type at position: " + cubeType);

            // Get all cubes in the group
            List<Vector2Int> group = gridGroups.GetGroup(gridPos.Value);

            Debug.Log("Group size: " + group.Count + " positions: " + string.Join(", ", group));

            // Only process if it's a valid group (2+ matching cubes)
            if (gridGroups.IsValidGroup(group))
            {
                Debug.Log("Valid group found, removing cubes");

                // Remove all cubes in the group
                foreach (Vector2Int pos in group)
                {
                    // Get cube game object before removing from storage
                    MonoBehaviour mb = gridManager.Storage.GetObjectAt(pos) as MonoBehaviour;
                    if (mb != null)
                    {
                        GameObject cubeGameObject = mb.gameObject;
                        Debug.Log("Removing cube at " + pos + ": " + cubeGameObject.name);

                        // Remove from grid storage
                        gridManager.Storage.RemoveObject(pos);

                        // Destroy the cube
                        Destroy(cubeGameObject);
                    }
                    else
                    {
                        Debug.LogError("Could not get MonoBehaviour at " + pos);
                    }
                }

                // Trigger falling after removing cubes
                Debug.Log("Cubes removed, triggering falling mechanism");
                if (fallingHandler != null)
                {
                    fallingHandler.ProcessFalling();
                }
                else
                {
                    Debug.LogError("FallingHandler reference is null, can't process falling!");
                }
            }
            else
            {
                Debug.Log("Not a valid group (needs 2+ matching cubes)");
            }
        }
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