using UnityEngine;
using System.Collections.Generic;

public class GridGroups
{
    private GridStorage gridStorage;
    private int gridWidth;
    private int gridHeight;

    // Constructor taking grid storage and dimensions
    public GridGroups(GridStorage storage, int width, int height)
    {
        gridStorage = storage;
        gridWidth = width;
        gridHeight = height;
    }



    // Get all positions in a group connected to the given position
    public List<Vector2Int> GetGroup(Vector2Int startPos)
    {
        objectColor targetType = gridStorage.GetColorAt(startPos);

        // If not a valid colored cube (r, g, b, y), return empty list
        if ((int)targetType == 99 ||
            (targetType != objectColor.r && targetType != objectColor.g &&
             targetType != objectColor.b && targetType != objectColor.y))
            return new List<Vector2Int>();

        // Use breadth-first search to find all connected cubes of same type
        List<Vector2Int> group = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(startPos);
        visited.Add(startPos);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            group.Add(current);

            // Check all four adjacent positions
            Vector2Int[] directions = {
                new Vector2Int(0, 1),  // Up
                new Vector2Int(1, 0),  // Right
                new Vector2Int(0, -1), // Down
                new Vector2Int(-1, 0)  // Left
            };

            foreach (Vector2Int dir in directions)
            {
                Vector2Int adjacentPos = current + dir;

                // Skip if outside grid bounds or already visited
                if (adjacentPos.x < 0 || adjacentPos.x >= gridWidth ||
                    adjacentPos.y < 0 || adjacentPos.y >= gridHeight ||
                    visited.Contains(adjacentPos))
                    continue;

                // If same type and is a valid cube, add to queue
                objectColor adjacentType = gridStorage.GetColorAt(adjacentPos);
                if (adjacentType == targetType && (int)adjacentType != 99)
                {
                    queue.Enqueue(adjacentPos);
                    visited.Add(adjacentPos);
                }
            }
        }

        return group;
    }

    // Also update the HasValidGroup method to be consistent:
    public bool HasValidGroup(Vector2Int position)
    {
        objectColor type = gridStorage.GetColorAt(position);

        // If not a colored cube (r, g, b, y) then not groupable
        if ((int)type == 99 ||
            (type != objectColor.r && type != objectColor.g &&
             type != objectColor.b && type != objectColor.y))
            return false;

        // Check each adjacent position
        Vector2Int[] directions = {
            new Vector2Int(0, 1),  // Up
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1, 0)  // Left
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int adjacentPos = position + dir;

            // Skip if outside grid bounds
            if (adjacentPos.x < 0 || adjacentPos.x >= gridWidth ||
                adjacentPos.y < 0 || adjacentPos.y >= gridHeight)
                continue;

            // If adjacent cube matches type, we have a valid group
            objectColor adjacentType = gridStorage.GetColorAt(adjacentPos);
            if (adjacentType == type && (int)adjacentType != 99)
                return true;
        }

        return false;
    }

    // Check if group is valid (at least 2 connected cubes)
    public bool IsValidGroup(List<Vector2Int> group)
    {
        return group.Count >= 2;
    }
    public bool IsValidGroupOfFour(List<Vector2Int> group)
        {
            return group.Count >= 4;
        }

    // Get all valid groups in the grid
    public List<List<Vector2Int>> GetAllGroups()
    {
        List<List<Vector2Int>> allGroups = new List<List<Vector2Int>>();
        HashSet<Vector2Int> processedPositions = new HashSet<Vector2Int>();

        // Check all positions in the grid
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2Int position = new Vector2Int(x, y);

                // Skip if already processed or not a valid cube type
                objectColor type = gridStorage.GetColorAt(position);
                if (processedPositions.Contains(position) ||
                    (type != objectColor.r && type != objectColor.g && type != objectColor.b && type != objectColor.y))
                    continue;

                // Get group at this position
                List<Vector2Int> group = GetGroup(position);

                // If valid group, add to results
                if (IsValidGroup(group))
                {
                    allGroups.Add(group);

                    // Mark all positions in this group as processed
                    foreach (Vector2Int groupPos in group)
                    {
                        processedPositions.Add(groupPos);
                    }
                }
                else
                {
                    // Still mark single cubes as processed
                    processedPositions.Add(position);
                }
            }
        }

        return allGroups;
    }

    // Find all groups that are eligible for rocket creation (4+ cubes)
    public List<Vector2Int> GetRocketEligiblePositions()
    {
        List<Vector2Int> eligiblePositions = new List<Vector2Int>();
        HashSet<Vector2Int> processedPositions = new HashSet<Vector2Int>();

        // Check all positions in the grid
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2Int position = new Vector2Int(x, y);

                // Skip if already processed or not a valid cube type
                objectColor type = gridStorage.GetColorAt(position);
                if (processedPositions.Contains(position) ||
                    (type != objectColor.r && type != objectColor.g &&
                     type != objectColor.b && type != objectColor.y))
                    continue;

                // Get group at this position
                List<Vector2Int> group = GetGroup(position);

                // If group has 4+ cubes, add all positions to eligible list
                if (group.Count >= 4)
                {
                    eligiblePositions.AddRange(group);

                    // Mark all positions in this group as processed
                    foreach (Vector2Int groupPos in group)
                    {
                        processedPositions.Add(groupPos);
                    }
                }
                else
                {
                    // Still mark all cubes in smaller groups as processed
                    foreach (Vector2Int groupPos in group)
                    {
                        processedPositions.Add(groupPos);
                    }
                }
            }
        }

        return eligiblePositions;
    }
}