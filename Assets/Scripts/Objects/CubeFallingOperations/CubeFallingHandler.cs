using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeFallingHandler : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    private GridGroups gridGroups;
    private GridStorage gridStorage;
    private bool isProcessingFalls = false;
    private int pendingFallAnimations = 0;

    public bool IsProcessing => isProcessingFalls || pendingFallAnimations > 0;

    private void Start()
    {
        gridStorage = gridManager.Storage;
        CheckForNewMatches();
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
        // Process all columns
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

        // Check if there are any new matches after falling
        CheckForNewMatches();
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

                // Find the nearest object above that can fall
                int targetY = y; // This is where we want to move the object to
                bool foundObjectToFall = false;

                for (int above = y + 1; above < gridManager.gridHeight; above++)
                {
                    Vector2Int abovePos = new Vector2Int(column, above);

                    if (gridStorage.HasObjectAt(abovePos))
                    {
                        // Check what type of object it is
                        IGridObject obj = gridStorage.GetObjectAt(abovePos);
                        CubeObject cube = obj as CubeObject;
                        RocketObject rocket = obj as RocketObject;
                        ObstacleObject obstacle = obj as ObstacleObject;

                        if (cube != null)
                        {
                            Debug.Log($"Found cube at {abovePos} that can fall to {currentPos}");
                            MoveObject(abovePos, new Vector2Int(column, targetY));
                            foundObjectToFall = true;
                            break;
                        }
                        else if (rocket != null)
                        {
                            Debug.Log($"Found rocket at {abovePos} that can fall to {currentPos}");
                            MoveObject(abovePos, new Vector2Int(column, targetY));
                            foundObjectToFall = true;
                            break;
                        }
                        else if (obstacle != null)
                        {
                            // Check if the obstacle can fall
                            if (obstacle is VaseObstacle) // Only vases can fall
                            {
                                Debug.Log($"Found vase at {abovePos} that can fall to {currentPos}");
                                MoveObject(abovePos, new Vector2Int(column, targetY));
                                foundObjectToFall = true;
                                break;
                            }
                            else
                            {
                                // If we hit a non-falling obstacle, we can't move past it
                                Debug.Log($"Found obstacle at {abovePos}, can't move past it");
                                break;
                            }
                        }
                    }
                }

                // If we found an object to fall, we need to continue checking this column
                if (foundObjectToFall)
                {
                    // Reprocess this row position as it might need to fall further
                    y--;
                }
            }
        }
    }

    private void MoveObject(Vector2Int fromPos, Vector2Int toPos)
    {
        Debug.Log($"Moving object from {fromPos} to {toPos}");

        // Get the object
        IGridObject gridObject = gridStorage.GetObjectAt(fromPos);
        MonoBehaviour mb = gridObject as MonoBehaviour;

        if (mb != null)
        {
            Debug.Log($"Found object to move: {mb.name}");

            // Start animation
            pendingFallAnimations++;
            Debug.Log($"Pending animations: {pendingFallAnimations}");

            AnimateObjectFall(mb, fromPos, toPos);

            // Update grid data
            string objectType = gridStorage.GetTypeAt(fromPos);
            Debug.Log($"Updating grid data, object type: {objectType}");

            gridStorage.StoreObject(toPos, gridObject, objectType);
            gridStorage.RemoveObject(fromPos);
        }
        else
        {
            Debug.LogError($"Failed to get object at position {fromPos}");
        }
    }

    private void AnimateObjectFall(MonoBehaviour obj, Vector2Int fromPos, Vector2Int toPos)
    {
        Vector2 startWorldPos = obj.transform.position;
        Vector2 endWorldPos = CalculateWorldPosition(toPos);

        StartCoroutine(AnimateObjectMovement(obj, startWorldPos, endWorldPos, 0.2f));
    }

    private IEnumerator AnimateObjectMovement(MonoBehaviour obj, Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            obj.transform.position = Vector2.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = endPos;

        // Update the sorting order for visual layering
        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            // Calculate grid position from world position
            Vector2 relativePos = endPos - (Vector2)gridManager.GridStartPos;
            int y = Mathf.RoundToInt(relativePos.y / gridManager.CellSize);
            renderer.sortingOrder = y;
        }

        pendingFallAnimations--;

        // If all animations are done, recheck for matches
        if (pendingFallAnimations == 0)
        {
            CheckForNewMatches();
        }
    }

    private Vector2 CalculateWorldPosition(Vector2Int gridPos)
    {
        return new Vector2(
            gridManager.GridStartPos.x + gridPos.x * gridManager.CellSize,
            gridManager.GridStartPos.y + gridPos.y * gridManager.CellSize
        );
    }

    public void CheckForNewMatches()
    {
        Debug.Log("Checking for new matches after falling");

        // Reset all cubes to normal sprites first
        ResetAllRocketHints();

        // Get positions eligible for rockets
        GridGroups gridGroups = new GridGroups(gridStorage, gridManager.gridWidth, gridManager.gridHeight);
        List<Vector2Int> rocketEligiblePositions = gridGroups.GetRocketEligiblePositions();

        // Set rocket hint sprites
        foreach (Vector2Int pos in rocketEligiblePositions)
        {
            IGridObject obj = gridStorage.GetObjectAt(pos);
            CubeObject cube = obj as CubeObject;
            if (cube != null)
            {
                cube.SetRocketHintVisible(true);
            }
        }
    }

    private void ResetAllRocketHints()
    {
        List<Vector2Int> allPositions = gridStorage.GetAllPositions();
        foreach (Vector2Int pos in allPositions)
        {
            IGridObject obj = gridStorage.GetObjectAt(pos);
            CubeObject cube = obj as CubeObject;
            if (cube != null)
            {
                cube.SetRocketHintVisible(false);
            }
        }
    }
}