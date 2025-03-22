using UnityEngine;
using System.Collections.Generic;

public class RocketInputHandler : MonoBehaviour
{
    private GridManager gridManager;
    private GridGroups gridGroups;
    [SerializeField] private CubeFallingHandler fallingHandler;


    void Start()
    {
        gridManager = Object.FindFirstObjectByType<GridManager>();
        gridGroups = new GridGroups(gridManager.Storage, gridManager.gridWidth, gridManager.gridHeight);

    }

    public void OnRocketClicked(RocketObject rocketObject)
    {
        Vector2Int? gridPos = FindGridPosition(rocketObject);

        if (gridPos.HasValue)
        {
            string rocketType = gridManager.Storage.GetTypeAt(gridPos.Value);

            // Get rocket game object before removing from storage
            MonoBehaviour mb = gridManager.Storage.GetObjectAt(gridPos.Value) as MonoBehaviour;
            if (mb != null)
            {
                GameObject rocketGameObject = mb.gameObject;

                // Remove from grid storage
                 gridManager.Storage.RemoveObject(gridPos.Value);

                // Destroy the rocket
                Destroy(rocketGameObject);

                // Apply damage in row/column based on rocket type
                // ApplyRocketDamage(gridPos.Value, rocketType);

                // Trigger falling after removing rocket
                Debug.Log("Rocket removed, triggering falling mechanism");
                if (fallingHandler != null)
                {
                    fallingHandler.ProcessFalling();
                }
                else
                {
                    Debug.LogError("FallingHandler reference is null, can't process falling!");
                }
            }
            else
            {
                Debug.LogError("Could not get MonoBehaviour at " + gridPos.Value);
            }
        }
    }

    private void ApplyRocketDamage(Vector2Int position, string rocketType)
    {
        // Check if horizontal or vertical rocket
        bool isHorizontal = rocketType == "hro";

        if (isHorizontal)
        {
            // Apply damage to entire row
            for (int x = 0; x < gridManager.gridWidth; x++)
            {
                Vector2Int targetPos = new Vector2Int(x, position.y);
                if (x != position.x) // Skip the rocket position itself (already removed)
                {
                    DamageGridCell(targetPos);
                }
            }
        }
        else
        {
            // Apply damage to entire column
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                Vector2Int targetPos = new Vector2Int(position.x, y);
                if (y != position.y) // Skip the rocket position itself (already removed)
                {
                    DamageGridCell(targetPos);
                }
            }
        }
    }

    private void DamageGridCell(Vector2Int position)
    {
        if (gridManager.Storage.HasObjectAt(position))
        {
            // Get the object at this position
            IGridObject gridObject = gridManager.Storage.GetObjectAt(position);
            MonoBehaviour mb = gridObject as MonoBehaviour;

            if (mb != null)
            {
                // Check if it's a damageable object
                IDamageable damageable = mb.GetComponent<IDamageable>();
                if (damageable != null && damageable.CanTakeDamage(DamageType.Rocket))
                {
                    damageable.TakeDamage(DamageType.Rocket, 1);
                    if (damageable.IsDestroyed)
                    {
                        gridManager.Storage.RemoveObject(position);
                        Destroy(mb.gameObject);
                    }
                }
                else
                {
                    // If not damageable, just remove it
                    gridManager.Storage.RemoveObject(position);
                    Destroy(mb.gameObject);
                }
            }
        }
    }

    private Vector2Int? FindGridPosition(RocketObject rocketObject)
    {
        List<Vector2Int> allPositions = gridManager.Storage.GetAllPositions();

        foreach (Vector2Int pos in allPositions)
        {
            IGridObject gridObject = gridManager.Storage.GetObjectAt(pos);

            // Cast IGridObject to MonoBehaviour to access gameObject, then check if it has the target component
            MonoBehaviour mb = gridObject as MonoBehaviour;
            if (mb != null && mb.GetComponent<RocketObject>() == rocketObject)
            {
                return pos;
            }
        }

        return null;
    }
}