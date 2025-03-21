using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeFallingHandler : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    private GridStorage gridStorage;
    private bool isProcessingFalls = false;
    private int pendingFallAnimations = 0;

    public bool IsProcessing => isProcessingFalls || pendingFallAnimations > 0;

    private void Start()
    {
        gridStorage = gridManager.Storage;
    }

    public void ProcessFalling()
    {
        Debug.Log("CubeFallingHandler.ProcessFalling() called");

        if (isProcessingFalls)
        {
            Debug.Log("Already processing falls, ignoring call");
            return;
        }

        Debug.Log("Starting falling process");
        isProcessingFalls = true;
        StartCoroutine(ProcessFallingCoroutine());
    }

    private IEnumerator ProcessFallingCoroutine()
    {
        // Instead of using the queue, process all columns
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            ProcessColumnFalling(x);
        }

        // Clear the empty spaces queue
        gridStorage.ClearEmptySpaces();
        isProcessingFalls = false;

        // Wait for all animations to complete
        while (pendingFallAnimations > 0)
        {
            yield return null;
        }

        Debug.Log("All falling animations completed");
    }

    private void ProcessColumnFalling(int column)
    {
        Debug.Log($"Processing column {column}");

        // Start from bottom row and work up
        for (int y = 0; y < gridManager.gridHeight; y++)
        {
            Vector2Int currentPos = new Vector2Int(column, y);

            // If this position is empty or marked as empty
            if (!gridStorage.HasObjectAt(currentPos) || gridStorage.GetTypeAt(currentPos) == "empty")
            {
                Debug.Log($"Found empty position at {currentPos}");

                // Find the nearest cube above that can fall
                for (int above = y + 1; above < gridManager.gridHeight; above++)
                {
                    Vector2Int abovePos = new Vector2Int(column, above);

                    if (gridStorage.HasObjectAt(abovePos))
                    {
                        // Is it a cube?
                        IGridObject obj = gridStorage.GetObjectAt(abovePos);
                        CubeObject cube = obj as CubeObject;

                        if (cube != null)
                        {
                            Debug.Log($"Found cube at {abovePos} that can fall to {currentPos}");

                            // Move this cube down
                            MoveCube(abovePos, currentPos);

                            // Don't continue scanning upward, we've filled this position
                            break;
                        }
                        else
                        {
                            Debug.Log($"Found non-cube object at {abovePos}, can't move past it");
                            // If we hit an obstacle, we can't move cubes past it
                            break;
                        }
                    }
                }
            }
        }
    }

    private void MoveCube(Vector2Int fromPos, Vector2Int toPos)
    {
        Debug.Log($"Moving cube from {fromPos} to {toPos}");

        // Get the cube object
        IGridObject gridObject = gridStorage.GetObjectAt(fromPos);
        CubeObject cube = gridObject as CubeObject;

        if (cube != null)
        {
            Debug.Log($"Found cube to move: {cube.name}");

            // Start animation
            pendingFallAnimations++;
            Debug.Log($"Pending animations: {pendingFallAnimations}");

            AnimateCubeFall(cube, fromPos, toPos);

            // Update grid data
            string cubeType = gridStorage.GetTypeAt(fromPos);
            Debug.Log($"Updating grid data, cube type: {cubeType}");

            gridStorage.StoreObject(toPos, gridObject, cubeType);
            gridStorage.RemoveObject(fromPos);
        }
        else
        {
            Debug.LogError($"Failed to get cube at position {fromPos}");
        }
    }

    private void AnimateCubeFall(CubeObject cube, Vector2Int fromPos, Vector2Int toPos)
    {
        Vector2 startWorldPos = cube.transform.position;
        Vector2 endWorldPos = CalculateWorldPosition(toPos);

        StartCoroutine(AnimateCubeMovement(cube, startWorldPos, endWorldPos, 0.2f));
    }

    private IEnumerator AnimateCubeMovement(CubeObject cube, Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            cube.transform.position = Vector2.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cube.transform.position = endPos;

        // Update the sorting order for visual layering
        SpriteRenderer renderer = cube.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            // Calculate grid position from world position
            Vector2 relativePos = endPos - (Vector2)gridManager.GridStartPos;
            int y = Mathf.RoundToInt(relativePos.y / gridManager.CellSize);
            renderer.sortingOrder = y;
        }

        pendingFallAnimations--;
    }

    private Vector2 CalculateWorldPosition(Vector2Int gridPos)
    {
        return new Vector2(
            gridManager.GridStartPos.x + gridPos.x * gridManager.CellSize,
            gridManager.GridStartPos.y + gridPos.y * gridManager.CellSize
        );
    }
}