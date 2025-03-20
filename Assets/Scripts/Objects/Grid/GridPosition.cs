using UnityEngine;

public class GridPosition
{
    public static Vector2 CalculateGridStartPosition(int gridWidth, int gridHeight, float cellSize)
    {
        float totalGridWidth = gridWidth * cellSize;
        float totalGridHeight = gridHeight * cellSize;

        // Calculate center point (assuming 0,0 is screen center)
        Vector2 screenCenter = new Vector2(0, 0);

        // Calculate top-left position of grid with offset to place center of the cell
        return new Vector2(
            screenCenter.x - (totalGridWidth / 2) + (cellSize / 2),
            screenCenter.y - (totalGridHeight / 2) + (cellSize / 2)
        );
    }
}