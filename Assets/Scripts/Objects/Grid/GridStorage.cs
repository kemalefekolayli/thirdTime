using UnityEngine;
using System.Collections.Generic;

public class GridStorage
{
    private Dictionary<Vector2Int, IGridObject> gridObjects = new Dictionary<Vector2Int, IGridObject>();
    private Dictionary<Vector2Int, string> gridTypes = new Dictionary<Vector2Int, string>();

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

    public bool HasObjectAt(Vector2Int position)
    {
        return gridObjects.ContainsKey(position);
    }

    public void RemoveObject(Vector2Int position)
    {
        if (gridObjects.ContainsKey(position))
        {
            gridObjects.Remove(position);
            gridTypes.Remove(position);
        }
    }

    public List<Vector2Int> GetAllPositions()
    {
        return new List<Vector2Int>(gridObjects.Keys);
    }

    public Dictionary<Vector2Int, IGridObject> GetAllObjects()
    {
        return new Dictionary<Vector2Int, IGridObject>(gridObjects);
    }
}