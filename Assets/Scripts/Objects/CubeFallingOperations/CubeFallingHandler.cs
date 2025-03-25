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
    private float checkDelay = 0.5f;
    [SerializeField] private GridFiller gridFiller;

    public bool IsProcessing => isProcessingFalls || pendingFallAnimations > 0;

    private void Start()
    {
        gridStorage = gridManager.Storage;
        CheckForNewMatches();
        gridFiller = FindFirstObjectByType<GridFiller>();
    }

    public void ProcessFalling()
    {
        Debug.Log("CubeFallingHandler.ProcessFalling() called");

        if (isProcessingFalls)
        {
            Debug.Log("Already processing falls, ignoring call");
            return;
        }


        isProcessingFalls = true;
        StartCoroutine(ProcessFallingCoroutine());
        OnFallingComplete();
    }

    private IEnumerator ProcessFallingCoroutine()
    {
        bool objectsMoved;
        int safetyCounter = 0;
        int maxIterations = 10; // Safety limit to prevent infinite loops

        do {
            objectsMoved = false;
            safetyCounter++;

            // Process all columns
            for (int x = 0; x < gridManager.gridWidth; x++)
            {
                bool columnChanged = ProcessColumnFalling(x);
                objectsMoved = objectsMoved || columnChanged;
            }

            // Wait for all animations to complete
            while (pendingFallAnimations > 0)
            {
                yield return null;
            }

            // Small delay to ensure stability
            yield return new WaitForSeconds(0.1f);

        } while (objectsMoved && safetyCounter < maxIterations);

        // Clear the empty spaces queue
        gridStorage.ClearEmptySpaces();
        isProcessingFalls = false;

        // Double-check that everything has settled
        yield return new WaitForSeconds(checkDelay);
        VerifyNoFloatingObjects();



        // Check if there are any new matches after falling
        CheckForNewMatches();
    }

    private bool ProcessColumnFalling(int column)
    {

        bool columnChanged = false;

        // Start from bottom row and work up
        for (int y = 0; y < gridManager.gridHeight; y++)
        {
            Vector2Int currentPos = new Vector2Int(column, y);

            // If this position is empty or marked as empty
            if (!gridStorage.HasObjectAt(currentPos) || gridStorage.GetTypeAt(currentPos) == "empty")
            {


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
                            MoveObject(abovePos, new Vector2Int(column, targetY));
                            foundObjectToFall = true;
                            columnChanged = true;
                            break;
                        }
                        else if (rocket != null)
                        {

                            MoveObject(abovePos, new Vector2Int(column, targetY));
                            foundObjectToFall = true;
                            columnChanged = true;
                            break;
                        }
                        else if (obstacle != null)
                        {
                            // Check if the obstacle can fall
                            if (obstacle is VaseObstacle) // Only vases can fall
                            {

                                MoveObject(abovePos, new Vector2Int(column, targetY));
                                foundObjectToFall = true;
                                columnChanged = true;
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }


                if (foundObjectToFall)
                {

                    y--;
                }
            }
        }

        return columnChanged;
    }

    private void MoveObject(Vector2Int fromPos, Vector2Int toPos)
    {

        // Get the object
        IGridObject gridObject = gridStorage.GetObjectAt(fromPos);
        MonoBehaviour mb = gridObject as MonoBehaviour;

        if (mb != null)
        {


            // Start animation
            pendingFallAnimations++;


            AnimateObjectFall(mb, fromPos, toPos);

            // Update grid data
            string objectType = gridStorage.GetTypeAt(fromPos);


            gridStorage.StoreObject(toPos, gridObject, objectType);
            gridStorage.RemoveObject(fromPos);
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

    private void VerifyNoFloatingObjects()
    {
        bool foundFloating = false;

        // Check each column from bottom to top
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            bool foundEmpty = false;

            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (!gridStorage.HasObjectAt(pos) || gridStorage.GetTypeAt(pos) == "empty")
                {
                    foundEmpty = true;
                }
                else if (foundEmpty)
                {
                    // We found an object after finding an empty space below it
                    IGridObject obj = gridStorage.GetObjectAt(pos);

                    // Check if this object can fall
                    CubeObject cube = obj as CubeObject;
                    RocketObject rocket = obj as RocketObject;
                    ObstacleObject obstacle = obj as ObstacleObject;

                    bool canFall = (cube != null) || (rocket != null) ||
                                  (obstacle != null && obstacle is VaseObstacle);

                    if (canFall)
                    {
                        foundFloating = true;
                        break;
                    }
                }
            }

            if (foundFloating) break;
        }

        if (foundFloating)
        {
            // Found floating objects, process falling again
            ProcessFalling();
        }
    }

    public void CheckForNewMatches()
    {


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

    void OnFallingComplete()
    {
        gridFiller.FillEmptySpaces();
    }
}