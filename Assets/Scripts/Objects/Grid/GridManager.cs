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
    [SerializeField] private CubeFallingHandler fallingHandler;

    public float CellSize => cellSize;
    public Vector2 GridStartPos => gridStartPos;
    public GridStorage Storage => gridStorage;

    void Start(){
        // Get current level number from LevelProgressManager
        int currentLevel = 1;
        if (LevelProgressManager.Instance != null) {
            currentLevel = LevelProgressManager.Instance.CurrentLevel;
        } else {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        }

        // Ensure level number is valid (1-10)
        currentLevel = Mathf.Clamp(currentLevel, 1, 10);

        // Load the level data for the current level
        LevelData levelData = LevelLoader.Instance.GetLevel(currentLevel);
        if (levelData == null)
        {
            Debug.LogError($"Level data for level {currentLevel} is NULL. Falling back to level 1.");
            levelData = LevelLoader.Instance.GetLevel(1);

            if (levelData == null) {
                Debug.LogError("Fallback level data is also NULL.");
                return;
            }
        }

        // Set grid dimensions from level data
        gridWidth = levelData.grid_width;
        gridHeight = levelData.grid_height;

        // Create parent object for grid elements
        GameObject parentObj = new GameObject("GridParent");
        gridParent = parentObj.transform;

        // Calculate cell size based on grid dimensions
        float gridMaxWidth = 4f;
        float gridMaxHeight = 4f;
        cellSize = Mathf.Min(gridMaxWidth / gridWidth, gridMaxHeight / gridHeight);

        // Calculate starting position
        gridStartPos = GridPosition.CalculateGridStartPosition(gridWidth, gridHeight, cellSize);

        // Load grid data and create objects
        cubeMatrix = LoadGridData(gridWidth, gridHeight, levelData.grid);

        // Check for matches after grid is created
        if (fallingHandler != null) {
            fallingHandler.CheckForNewMatches();
        }
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