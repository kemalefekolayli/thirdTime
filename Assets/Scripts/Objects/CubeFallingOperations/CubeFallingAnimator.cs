using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeFallingAnimator : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private FactoryManager factoryManager;

    private int pendingAnimations = 0;
    private float fallSpeed = 8f;
    private string[] colorOptions = { "r", "g", "b", "y" };

    public bool IsAnimating => pendingAnimations > 0;

    void Start()
    {
        if (gridManager == null) gridManager = FindFirstObjectByType<GridManager>();
        if (factoryManager == null) factoryManager = FindFirstObjectByType<FactoryManager>();
    }

    public void FillEmptyCellsWithAnimation()
    {
        StartCoroutine(SimpleFallingFill());
    }

    private IEnumerator SimpleFallingFill()
    {
        // First find and fill all empty cells
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (!gridManager.Storage.HasObjectAt(pos) ||
                    gridManager.Storage.GetTypeAt(pos) == "empty")
                {
                    // First create the cube at its final position with no visuals
                    string randomColor = colorOptions[Random.Range(0, colorOptions.Length)];
                    Vector2 finalWorldPos = new Vector2(
                        gridManager.GridStartPos.x + pos.x * gridManager.CellSize,
                        gridManager.GridStartPos.y + pos.y * gridManager.CellSize
                    );

                    ObjectFactory<IGridObject> factory = factoryManager.GetFactory(randomColor);
                    if (factory != null)
                    {
                        IGridObject newCube = factory.CreateObject(
                            finalWorldPos,
                            gridManager.transform,
                            gridManager.CellSize,
                            gridManager,
                            pos
                        );

                        if (newCube != null)
                        {
                            // Register in grid data
                            gridManager.Storage.StoreObject(pos, newCube, randomColor);

                            // Make invisible initially
                            MonoBehaviour mb = newCube as MonoBehaviour;
                            if (mb != null)
                            {
                                SpriteRenderer renderer = mb.GetComponent<SpriteRenderer>();
                                if (renderer != null)
                                {
                                    renderer.enabled = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Then animate all columns one by one
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            // For each column, find all filled cells and animate them
            List<(MonoBehaviour, Vector2Int)> cubesInColumn = new List<(MonoBehaviour, Vector2Int)>();

            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (gridManager.Storage.HasObjectAt(pos))
                {
                    IGridObject obj = gridManager.Storage.GetObjectAt(pos);
                    MonoBehaviour mb = obj as MonoBehaviour;

                    if (mb != null)
                    {
                        SpriteRenderer renderer = mb.GetComponent<SpriteRenderer>();
                        if (renderer != null && !renderer.enabled)
                        {
                            // This is a new cube we created
                            cubesInColumn.Add((mb, pos));
                        }
                    }
                }
            }

            // Sort from bottom to top
            cubesInColumn.Sort((a, b) => a.Item2.y.CompareTo(b.Item2.y));

            // Animate them falling
            for (int i = 0; i < cubesInColumn.Count; i++)
            {
                MonoBehaviour cube = cubesInColumn[i].Item1;
                Vector2Int targetPos = cubesInColumn[i].Item2;

                // Calculate starting position
                Vector2Int startGridPos = new Vector2Int(x, targetPos.y + gridManager.gridHeight);
                Vector2 startWorldPos = new Vector2(
                    gridManager.GridStartPos.x + startGridPos.x * gridManager.CellSize,
                    gridManager.GridStartPos.y + startGridPos.y * gridManager.CellSize
                );

                Vector2 endWorldPos = new Vector2(
                    gridManager.GridStartPos.x + targetPos.x * gridManager.CellSize,
                    gridManager.GridStartPos.y + targetPos.y * gridManager.CellSize
                );

                // Make visible
                SpriteRenderer renderer = cube.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                }

                // Set initial position
                cube.transform.position = startWorldPos;

                // Start animation
                pendingAnimations++;
                StartCoroutine(AnimateCube(cube, startWorldPos, endWorldPos));

                // Small delay between cubes in same column
                yield return new WaitForSeconds(0.05f);
            }

            // Small delay between columns
            yield return new WaitForSeconds(0.05f);
        }

        // Wait for all animations to complete
        while (pendingAnimations > 0)
        {
            yield return null;
        }

        // One final frame to ensure everything is settled
        yield return new WaitForEndOfFrame();

        // Let everyone know we're done
        GridEvents.TriggerFillingComplete();
    }

    private IEnumerator AnimateCube(MonoBehaviour cube, Vector2 startPos, Vector2 endPos)
    {
        float distance = Vector2.Distance(startPos, endPos);
        float duration = distance / fallSpeed;
        float elapsedTime = 0;

        while (elapsedTime < duration && cube != null)
        {
            cube.transform.position = Vector2.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (cube != null)
        {
            cube.transform.position = endPos;
        }

        pendingAnimations--;
    }
}