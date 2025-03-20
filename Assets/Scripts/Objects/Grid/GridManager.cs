using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

    private Dictionary<Vector2Int, IGridObject> gridState = new Dictionary<Vector2Int, IGridObject>();
    public int gridWidth;
    public int gridHeight;
    private float cellSize;
    private Vector2 gridStartPos;
    private string[,] cubeMatrix;

    void Start(){
    LevelData level1 = LevelLoader.Instance.GetLevel(1);
            if (level1 == null)
            {
                Debug.LogError("Level data is NULL.");
                return;
            }

    gridWidth = level1.grid_width;
    gridHeight = level1.grid_height;
    cubeMatrix = LoadGridData(gridWidth, gridHeight, level1.grid);
    }

    public string[,] LoadGridData(int gridWidth, int gridHeight, string[] gridData ){
    string[,] gridArray = new string[gridWidth, gridHeight];
    int index = 0;
    for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    gridArray[x, y] = gridData[index];
                    index++;
                }
            }
    return gridArray;
    }
}