using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Responsible for detecting adjacent rockets to enable combo explosions
/// </summary>
public class RocketComboDetector : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    private void Awake()
    {
        if (gridManager == null)
        {
            gridManager = Object.FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                Debug.LogError("RocketComboDetector: GridManager reference not found!");
            }
        }
    }


    public List<Vector2Int> FindAdjacentRockets(Vector2Int position)
    {
        List<Vector2Int> adjacentRockets = new List<Vector2Int>();

        // Define the four adjacent directions
        Vector2Int[] directions = {
            new Vector2Int(0, 1),  // Up
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1, 0)  // Left
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int adjacentPos = position + dir;

            // Check if the position is valid and contains a rocket
            if (IsValidGridPosition(adjacentPos) && IsRocket(adjacentPos))
            {
                adjacentRockets.Add(adjacentPos);
            }
        }

        return adjacentRockets;
    }


    public bool IsRocket(Vector2Int position)
    {
        if (!gridManager.Storage.HasObjectAt(position))
            return false;

        string objectType = gridManager.Storage.GetTypeAt(position);
        return objectType == "hro" || objectType == "vro";
    }


    private bool IsValidGridPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridManager.gridWidth &&
               position.y >= 0 && position.y < gridManager.gridHeight;
    }


    public bool IsHorizontalRocket(Vector2Int position)
    {
        if (!IsRocket(position))
            return false;

        string objectType = gridManager.Storage.GetTypeAt(position);
        return objectType == "hro";
    }

    public bool HasRocketCombo(Vector2Int position)
    {
        return FindAdjacentRockets(position).Count > 0;
    }
}