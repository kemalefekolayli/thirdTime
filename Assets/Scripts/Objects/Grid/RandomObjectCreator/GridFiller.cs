using UnityEngine;
using System.Collections;

public class GridFiller : MonoBehaviour // THIS IS BROKEN
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private FactoryManager factoryManager;

    private string[] colorOptions = { "r", "g", "b", "y" };
    private bool isProcessing = false;

    public bool IsProcessing => isProcessing;

    public void FillEmptySpaces()
    {
        isProcessing = true;
        StartCoroutine(FillEmptySpacesCoroutine());
    }

    private IEnumerator FillEmptySpacesCoroutine()
    {
        // Find all empty spaces and fill them
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (!gridManager.Storage.HasObjectAt(pos) ||
                    gridManager.Storage.GetTypeAt(pos) == "empty")
                {
                    CreateRandomCube(pos);
                    yield return new WaitForSeconds(0.05f); // Small delay for visual effect
                }
            }
        }

        isProcessing = false;
    }

    private void CreateRandomCube(Vector2Int gridPos)
    {
        // Select random color
        string randomColor = colorOptions[Random.Range(0, colorOptions.Length)];

        // Get factory
        ObjectFactory<IGridObject> factory = factoryManager.GetFactory(randomColor);

        if (factory != null)
        {
            // Calculate world position
            Vector2 worldPos = new Vector2(
                gridManager.GridStartPos.x + gridPos.x * gridManager.CellSize,
                gridManager.GridStartPos.y + gridPos.y * gridManager.CellSize
            );

            // Create cube
            IGridObject newCube = factory.CreateObject(
                worldPos,
                gridManager.transform,
                gridManager.CellSize,
                gridManager,
                gridPos
            );

            // Store in grid
            if (newCube != null)
            {
                gridManager.Storage.StoreObject(gridPos, newCube, randomColor);
            }
        }
    }
}