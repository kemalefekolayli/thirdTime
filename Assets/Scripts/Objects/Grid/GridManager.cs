using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

    private Dictionary<Vector2Int, IGridObject> gridState = new Dictionary<Vector2Int, IGridObject>();
    private Transform gridParent;
    public int gridWidth;
    public int gridHeight;
    private float cellSize;
    private Vector2 gridStartPos;
    private string[,] cubeMatrix;
    [SerializeField] private FactoryManager factoryManager;

    void Start(){
    LevelData level1 = LevelLoader.Instance.GetLevel(1);
            if (level1 == null)
            {
                Debug.LogError("Level data is NULL.");
                return;
            }
    Debug.LogError(string.Join(", ", level1.grid));
    gridWidth = level1.grid_width;
    gridHeight = level1.grid_height;

    GameObject parentObj = new GameObject("GridParent");
    gridParent = parentObj.transform;

            // ✅ Dynamic grid scaling
    float gridMaxWidth = 4f;
    float gridMaxHeight = 4f;
    cellSize = Mathf.Min(gridMaxWidth / gridWidth, gridMaxHeight / gridHeight);
    cubeMatrix = LoadGridData(gridWidth, gridHeight, level1.grid);
    Vector2 screenCenter = new Vector2(0, 0);
    gridStartPos = new Vector2(
       screenCenter.x - ((gridWidth * cellSize) / 2) + (cellSize / 2),
       screenCenter.y - ((gridHeight * cellSize) / 2) + (cellSize / 2)
            );

    }

    public string[,] LoadGridData(int gridWidth, int gridHeight, string[] gridData ){
    string[,] gridArray = new string[gridWidth, gridHeight];
    int index = 0;
    for (int y = 0; y < gridHeight; y++) {
        for (int x = 0; x < gridWidth; x++) {
            gridArray[x, y] = gridData[index];

            // Get the corresponding factory
            ObjectFactory<IGridObject>  factory = factoryManager.GetFactory(gridData[index]);

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

                // Store the created object in grid state
                if (gridObject != null) {
                    Debug.LogError("burdayım");
                    gridState[new Vector2Int(x, y)] = gridObject;
                }
            }

            index++;
        }
    }
    return gridArray;
    }


}