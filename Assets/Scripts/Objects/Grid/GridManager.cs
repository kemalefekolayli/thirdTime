using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

    private GridStorage gridStorage = new GridStorage();
    private Transform gridParent;
    public int gridWidth;
    public int gridHeight;
    private float cellSize;
    private Vector2 gridStartPos;
    private string[,] cubeMatrix;
    [SerializeField] private FactoryManager factoryManager;
    public float CellSize => cellSize;
    public Vector2 GridStartPos => gridStartPos;
    [SerializeField] private CubeFallingHandler fallingHandler;


    public GridStorage Storage => gridStorage;

    void Start(){
        LevelData level3 = LevelLoader.Instance.GetLevel(3);
        if (level3 == null)
        {
            Debug.LogError("Level data is NULL.");
            return;
        }

        gridWidth = level3.grid_width;
        gridHeight = level3.grid_height;

        GameObject parentObj = new GameObject("GridParent");
        gridParent = parentObj.transform;

        float gridMaxWidth = 4f;
        float gridMaxHeight = 4f;
        cellSize = Mathf.Min(gridMaxWidth / gridWidth, gridMaxHeight / gridHeight);

        gridStartPos = GridPosition.CalculateGridStartPosition(gridWidth, gridHeight, cellSize);

        cubeMatrix = LoadGridData(gridWidth, gridHeight, level3.grid);

        // Initialize GridGroups after grid is created
        GridGroups groupChecker = new GridGroups(gridStorage, gridWidth, gridHeight);

        fallingHandler.CheckForNewMatches();
    }

    public string[,] LoadGridData(int gridWidth, int gridHeight, string[] gridData) {
        string[,] gridArray = new string[gridWidth, gridHeight];
        int index = 0;
        for (int y = 0; y < gridHeight; y++) {
            for (int x = 0; x < gridWidth; x++) {
                gridArray[x, y] = gridData[index];

                // Get the corresponding factory
                ObjectFactory<IGridObject> factory = factoryManager.GetFactory(gridData[index]);

                if (factory != null) {
                    // Calculate world position from grid position
                    Vector2 worldPos = new Vector2(gridStartPos.x + x * cellSize, gridStartPos.y + y * cellSize);

                    // Create the object
                    IGridObject gridObject = factory.CreateObject(
                        worldPos,
                        gridParent,
                        cellSize,
                        this,
                        new Vector2Int(x, y)
                    );

                    // Store the created object in grid storage
                    if (gridObject != null) {
                        gridStorage.StoreObject(new Vector2Int(x, y), gridObject, gridData[index]);
                    }
                }

                index++;
            }
        }
        return gridArray;
    }

    public Vector2 GetGridStartPos(){
    return this.gridStartPos;
     }

    public float GetCellSize(){
    return this.cellSize;
    }
}