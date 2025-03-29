using UnityEngine;
using System.Collections.Generic;

public class GridStorage
{
    private Dictionary<Vector2Int, IGridObject> gridObjects = new Dictionary<Vector2Int, IGridObject>();
    private Dictionary<Vector2Int, string> gridTypes = new Dictionary<Vector2Int, string>();
    private Queue<Vector2Int> EmptySpaces = new Queue<Vector2Int>();
    private IGridObject EmptyObject = new EmptyObject();

    public void StoreObject(Vector2Int position, IGridObject gridObject, string type)
    {
        gridObjects[position] = gridObject;
        gridTypes[position] = type;
    }

    public IGridObject GetObjectAt(Vector2Int position)
    {
        if (gridObjects.ContainsKey(position))
            return gridObjects[position];
        return null;
    }

    public string GetTypeAt(Vector2Int position)
    {
        if (gridTypes.ContainsKey(position))
            return gridTypes[position];
        return null;
    }

    public objectColor GetColorAt(Vector2Int position)
    {
        if (gridObjects.ContainsKey(position))
        {
            // Cast to CubeObject to access the color
            CubeObject cube = gridObjects[position] as CubeObject;
            if (cube != null)
            {
                return cube.GetCubeColor();
            }
            else
            {
                // For non-cube objects like obstacles, return an invalid color value
                return (objectColor)99; // Using a value outside the enum range
            }
        }

        // Default return for empty cells
        return (objectColor)99;
    }

    public bool HasObjectAt(Vector2Int position)
    {
        return gridObjects.ContainsKey(position);
    }

    public void RemoveObject(Vector2Int position)
    {
        if (gridObjects.ContainsKey(position))
        {
            gridObjects[position] = EmptyObject;
            gridTypes[position] = "empty";
            EmptySpaces.Enqueue(position);
        }
    }
    public Queue<Vector2Int> GetEmptySpaces(){
    return this.EmptySpaces;
    }

    public void ClearEmptySpaces(){
    this.EmptySpaces.Clear();
    }
    public List<Vector2Int> GetAllPositions()
    {
        return new List<Vector2Int>(gridObjects.Keys);
    }

    public Dictionary<Vector2Int, IGridObject> GetAllObjects()
    {
        return new Dictionary<Vector2Int, IGridObject>(gridObjects);
    }
    public Dictionary<Vector2Int, string> GetAllObjectTypes()
        {
            return new Dictionary<Vector2Int, string>(gridTypes);
        }
}