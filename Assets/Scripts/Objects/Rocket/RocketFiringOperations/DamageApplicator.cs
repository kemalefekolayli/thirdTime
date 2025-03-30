using UnityEngine;
using System.Collections.Generic;

public class DamageApplicator : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    private GoalTracker goalTracker;

    private void Awake()
    {
        if (gridManager == null)
        {
            gridManager = Object.FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                Debug.LogError("DamageApplicator: GridManager reference not found!");
            }
        }

        // Get reference to GoalTracker
        goalTracker = Object.FindFirstObjectByType<GoalTracker>();
        if (goalTracker == null)
        {
            Debug.LogWarning("DamageApplicator: GoalTracker not found - obstacle tracking will not work correctly");
        }
    }

    public bool ApplyDamage(Vector2Int position, DamageType damageType)
    {
        bool objectDestroyed = false;

        if (gridManager.Storage.HasObjectAt(position))
        {
            // Get the object type before potentially destroying it
            string objectType = gridManager.Storage.GetTypeAt(position);

            // Get the object at this position
            IGridObject gridObject = gridManager.Storage.GetObjectAt(position);
            MonoBehaviour mb = gridObject as MonoBehaviour;

            if (mb != null)
            {
                // Check if it's a damageable object (obstacles)
                IDamageable damageable = mb.GetComponent<IDamageable>();
                if (damageable != null && damageable.CanTakeDamage(damageType))
                {
                    damageable.TakeDamage(damageType, 1);
                    if (damageable.IsDestroyed)
                    {
                        // Obstacle yok edildiğinde GoalTracker'a bildir
                        if (objectType == "v" || objectType == "s" || objectType == "bo")
                        {

                            if (goalTracker != null)
                            {
                                goalTracker.ObstacleDestroyed(objectType);
                                Debug.Log($"Obstacle destroyed: {objectType} at {position}");
                            }
                        }

                        gridManager.Storage.RemoveObject(position);
                        Destroy(mb.gameObject);
                        objectDestroyed = true;
                    }
                }
                else
                {
                    // If not damageable, it's a cube or another rocket - just remove it
                    gridManager.Storage.RemoveObject(position);
                    Destroy(mb.gameObject);
                    objectDestroyed = true;
                }
            }
        }

        return objectDestroyed;
    }

    public List<Vector2Int> ApplyLineDamage(Vector2Int startPosition, bool isHorizontal, int startOffset, int length)
    {
        List<Vector2Int> affectedPositions = new List<Vector2Int>();

        int start = startOffset;
        int end = startOffset + length;

        if (isHorizontal)
        {
            // Apply to row
            for (int x = start; x < end; x++)
            {
                if (x >= 0 && x < gridManager.gridWidth)
                {
                    Vector2Int targetPos = new Vector2Int(x, startPosition.y);
                    bool affected = ApplyDamage(targetPos, DamageType.Rocket);
                    if (affected)
                    {
                        affectedPositions.Add(targetPos);
                    }
                }
            }
        }
        else
        {
            // Apply to column
            for (int y = start; y < end; y++)
            {
                if (y >= 0 && y < gridManager.gridHeight)
                {
                    Vector2Int targetPos = new Vector2Int(startPosition.x, y);
                    bool affected = ApplyDamage(targetPos, DamageType.Rocket);
                    if (affected)
                    {
                        affectedPositions.Add(targetPos);
                    }
                }
            }
        }

        return affectedPositions;
    }

    public List<Vector2Int> ApplyAreaDamage(Vector2Int center, int radius)
    {
        List<Vector2Int> affectedPositions = new List<Vector2Int>();

        for (int x = center.x - radius; x <= center.x + radius; x++)
        {
            for (int y = center.y - radius; y <= center.y + radius; y++)
            {
                if (x >= 0 && x < gridManager.gridWidth && y >= 0 && y < gridManager.gridHeight)
                {
                    Vector2Int targetPos = new Vector2Int(x, y);
                    bool affected = ApplyDamage(targetPos, DamageType.Rocket);
                    if (affected)
                    {
                        affectedPositions.Add(targetPos);
                    }
                }
            }
        }

        return affectedPositions;
    }
}