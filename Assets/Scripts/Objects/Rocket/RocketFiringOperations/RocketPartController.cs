using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls a single explosion part that travels across the grid
/// </summary>
public class RocketPartController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private ParticleSystem explosionEffect;

    private Vector2Int gridDirection;
    private Vector2Int currentGridPosition;
    private GridManager gridManager;
    private DamageApplicator damageApplicator;
    private bool isMoving = false;
    private List<Vector2Int> affectedPositions = new List<Vector2Int>();

    public delegate void OnRocketPartFinished(List<Vector2Int> affectedPositions);
    public event OnRocketPartFinished OnFinished;

    /// <summary>
    /// Initialize the rocket part with its direction and starting position
    /// </summary>
    public void Initialize(GridManager gridManager, Vector2Int startGridPosition, Vector2Int direction, DamageApplicator damageApplicator)
    {
        this.gridManager = gridManager;
        this.currentGridPosition = startGridPosition;
        this.gridDirection = direction;
        this.damageApplicator = damageApplicator;

        // Set rotation based on direction
        if (direction.x != 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90); // Horizontal
        }

        // Position at start grid position
        transform.position = GridToWorldPosition(startGridPosition);

        // Start moving
        StartCoroutine(MoveAlongGrid());
    }

    /// <summary>
    /// Converts grid position to world position
    /// </summary>
    private Vector2 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector2(
            gridManager.GridStartPos.x + gridPos.x * gridManager.CellSize,
            gridManager.GridStartPos.y + gridPos.y * gridManager.CellSize
        );
    }

    /// <summary>
    /// Apply damage to the target position and handle rocket chain reactions
    /// </summary>
    private bool DamagePosition(Vector2Int position)
    {
        if (!gridManager.Storage.HasObjectAt(position))
            return false;

        IGridObject gridObject = gridManager.Storage.GetObjectAt(position);
        MonoBehaviour mb = gridObject as MonoBehaviour;

        if (mb == null)
            return false;

        // Check if it's another rocket
        RocketObject rocket = mb.GetComponent<RocketObject>();
        if (rocket != null)
        {
            // We already have the position, no need to search again
            string rocketType = gridManager.Storage.GetTypeAt(position);
            gridManager.Storage.RemoveObject(position);
            Destroy(mb.gameObject);

            // Explode this rocket too - make sure we're using FindFirstObjectByType
            RocketExplosionManager explosionManager = Object.FindFirstObjectByType<RocketExplosionManager>();
            if (explosionManager != null)
            {
                explosionManager.ExplodeRocket(position, rocketType);
            }

            return true;
        }

        // If not a rocket, apply normal damage
        return damageApplicator.ApplyDamage(position, DamageType.Rocket);
    }

        // Play explosion effect at the end
        if (explosionEffect != null)
        {
            explosionEffect.Play();
            yield return new WaitForSeconds(explosionEffect.main.duration);
        }

        // Notify that this part has finished
        if (OnFinished != null)
        {
            OnFinished(affectedPositions);
        }

        // Destroy this part
        Destroy(gameObject);
    }

    /// <summary>
    /// Determines if a position is within the grid bounds
    /// </summary>
    private bool IsValidGridPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridManager.gridWidth &&
               position.y >= 0 && position.y < gridManager.gridHeight;
    }

    /// <summary>
    /// Stop this rocket part and destroy it
    /// </summary>
    public void StopAndDestroy()
    {
        if (isMoving)
        {
            StopAllCoroutines();
            if (OnFinished != null)
            {
                OnFinished(affectedPositions);
            }
            Destroy(gameObject);
        }
    }
}