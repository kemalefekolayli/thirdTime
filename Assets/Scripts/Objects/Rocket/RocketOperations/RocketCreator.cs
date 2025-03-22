using UnityEngine;

public class RocketCreator : MonoBehaviour
{
    [SerializeField] private VerticalRocketFactory verticalRocketFactory;
    [SerializeField] private HorizontalRocketFactory horizontalRocketFactory;
    [SerializeField] private GridManager gridManager;

    private void Start()
    {}


    public IGridObject CreateRocket(Vector2Int gridPosition)
    {
        // Randomly choose horizontal or vertical rocket
        bool createHorizontal = Random.value > 0.5f;

        // Get the appropriate factory
        ObjectFactory<IGridObject> factory = createHorizontal ?
            horizontalRocketFactory as ObjectFactory<IGridObject> :
            verticalRocketFactory as ObjectFactory<IGridObject>;

        if (factory == null)
        {
            Debug.LogError("Rocket factory is null!");
            return null;
        }

        // Calculate world position from grid position
        Vector2 worldPos = new Vector2(
            gridManager.GridStartPos.x + gridPosition.x * gridManager.CellSize,
            gridManager.GridStartPos.y + gridPosition.y * gridManager.CellSize
        );

        // Create the rocket object
        IGridObject rocketObj = factory.CreateObject(
            worldPos,
            gridManager.transform,
            gridManager.CellSize,
            gridManager,
            gridPosition
        );

        // Store in grid
        if (rocketObj != null)
        {
            string rocketType = createHorizontal ? "hro" : "vro";
            gridManager.Storage.StoreObject(gridPosition, rocketObj, rocketType);
            return rocketObj;
        }
        else
        {
            Debug.LogError("Failed to create rocket object");
            return null;
        }
    }
}