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

    // This is the main entry point called after matches are removed
    public void ProcessFalling()
    {
        if (isProcessingFalls) return;

        isProcessingFalls = true;
        StartCoroutine(ProcessFallingCoroutine());
    }

    private IEnumerator ProcessFallingCoroutine()
    {
        // Get empty spaces and organize by column
        Queue<Vector2Int> emptySpaces = new Queue<Vector2Int>(gridStorage.GetEmptySpaces());
        Dictionary<int, List<Vector2Int>> columnEmptySpaces = GroupEmptySpacesByColumn(emptySpaces);

        // Process each column's empty spaces
        foreach (int column in columnEmptySpaces.Keys)
        {
            ProcessColumnFalling(column, columnEmptySpaces[column]);
        }

        // Clear the empty spaces queue
        gridStorage.ClearEmptySpaces();
        isProcessingFalls = false;

        // Wait for all animations to complete
        while (pendingFallAnimations > 0)
        {
            yield return null;
        }

        // Check for new matches that may have formed after falling
        // gridManager.CheckForMatches();
    }

    private Dictionary<int, List<Vector2Int>> GroupEmptySpacesByColumn(Queue<Vector2Int> emptySpaces)
    {
        Dictionary<int, List<Vector2Int>> columnEmptySpaces = new Dictionary<int, List<Vector2Int>>();

        while (emptySpaces.Count > 0)
        {
            Vector2Int emptyPos = emptySpaces.Dequeue();

            if (!columnEmptySpaces.ContainsKey(emptyPos.x))
            {
                columnEmptySpaces[emptyPos.x] = new List<Vector2Int>();
            }

            columnEmptySpaces[emptyPos.x].Add(emptyPos);
        }

        // Sort each column's empty spaces from bottom to top
        foreach (var column in columnEmptySpaces.Keys)
        {
            columnEmptySpaces[column].Sort((a, b) => a.y.CompareTo(b.y));
        }

        return columnEmptySpaces;
    }

    private void ProcessColumnFalling(int column, List<Vector2Int> emptyPositions)
    {
        foreach (Vector2Int emptyPos in emptyPositions)
        {
            // Find the nearest fallable object above this empty position
            Vector2Int? fallObjectPos = FindFallableObjectAbove(column, emptyPos.y);

            if (fallObjectPos.HasValue)
            {
                MoveCube(fallObjectPos.Value, emptyPos);
            }
        }
    }

    private Vector2Int? FindFallableObjectAbove(int column, int startRow)
    {
        for (int row = startRow + 1; row < gridManager.gridHeight; row++)
        {
            Vector2Int pos = new Vector2Int(column, row);

            if (gridStorage.HasObjectAt(pos))
            {
                // Check if this is a cube (which can fall)
                CubeObject cube = gridStorage.GetObjectAt(pos) as CubeObject;
                if (cube != null)
                {
                    return pos;
                }
                // If we hit an obstacle that doesn't fall, stop searching this column
                else
                {
                    break;
                }
            }
        }

        return null;
    }

    private void MoveCube(Vector2Int fromPos, Vector2Int toPos)
    {
        // Get the cube object
        IGridObject gridObject = gridStorage.GetObjectAt(fromPos);
        CubeObject cube = gridObject as CubeObject;

        if (cube != null)
        {
            // Start animation
            pendingFallAnimations++;
            AnimateCubeFall(cube, fromPos, toPos);

            // Update grid data
            string cubeType = gridStorage.GetTypeAt(fromPos);
            gridStorage.StoreObject(toPos, gridObject, cubeType);
            gridStorage.RemoveObject(fromPos);
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
            renderer.sortingOrder = cube.transform.position.y < 0 ?
                (int)(-cube.transform.position.y * 100) : 0;
        }

        pendingFallAnimations--;
    }

    private Vector2 CalculateWorldPosition(Vector2Int gridPos)
    {
        return new Vector2(
            gridManager.GetGridStartPos().x + gridPos.x * gridManager.GetCellSize(),
            gridManager.GetGridStartPos().y + gridPos.y * gridManager.GetCellSize()
        );
    }
}