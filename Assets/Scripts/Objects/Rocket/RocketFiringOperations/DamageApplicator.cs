using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Responsible for applying damage to grid objects during rocket explosions
/// </summary>
public class DamageApplicator : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

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
    }

    /// <summary>
    /// Apply damage to a specific grid cell
    /// </summary>
    public bool ApplyDamage(Vector2Int position, DamageType damageType)
    {
        bool objectDestroyed = false;

        if (gridManager.Storage.HasObjectAt(position))
        {
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

    /// <summary>
    /// Apply damage to entire row or column based on direction
    /// </summary>
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

    /// <summary>
    /// Apply damage in a 3x3 area for combo explosions
    /// </summary>
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