using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private Vector2 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector2(
            gridManager.GridStartPos.x + gridPos.x * gridManager.CellSize,
            gridManager.GridStartPos.y + gridPos.y * gridManager.CellSize
        );
    }

    private IEnumerator MoveAlongGrid()
    {
        isMoving = true;
        Vector2Int targetGridPosition = currentGridPosition + gridDirection;

        while (IsValidGridPosition(targetGridPosition))
        {
            // Check if there's a rocket at the target position
            bool isRocket = CheckForRocket(targetGridPosition);
            if (isRocket)
            {
                // Rocket chain reaction handled in CheckForRocket
                affectedPositions.Add(targetGridPosition);
                break; // Stop after hitting another rocket
            }

            // Apply damage to the target position
            bool affected = damageApplicator.ApplyDamage(targetGridPosition, DamageType.Rocket);
            if (affected)
            {
                affectedPositions.Add(targetGridPosition);
            }

            // Move to the target position
            Vector2 startWorldPos = transform.position;
            Vector2 targetWorldPos = GridToWorldPosition(targetGridPosition);

            float journeyLength = Vector2.Distance(startWorldPos, targetWorldPos);
            float moveTime = journeyLength / moveSpeed;
            float elapsedTime = 0f;

            while (elapsedTime < moveTime)
            {
                transform.position = Vector2.Lerp(startWorldPos, targetWorldPos, elapsedTime / moveTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Update current position and get next target
            currentGridPosition = targetGridPosition;
            targetGridPosition = currentGridPosition + gridDirection;
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

    private bool CheckForRocket(Vector2Int position)
    {
        if (!gridManager.Storage.HasObjectAt(position))
            return false;

        IGridObject gridObject = gridManager.Storage.GetObjectAt(position);
        MonoBehaviour mb = gridObject as MonoBehaviour;

        if (mb == null)
            return false;

        // Check if it's a rocket
        RocketObject rocket = mb.GetComponent<RocketObject>();
        if (rocket != null)
        {
            // Get the rocket type before removing it
            string rocketType = gridManager.Storage.GetTypeAt(position);

            // Remove the rocket from the grid
            gridManager.Storage.RemoveObject(position);
            Destroy(mb.gameObject);

            // Trigger explosion of this rocket
            RocketExplosionManager explosionManager = Object.FindFirstObjectByType<RocketExplosionManager>();
            if (explosionManager != null)
            {
                explosionManager.ExplodeRocket(position, rocketType);
            }

            return true;
        }

        return false;
    }

    private bool IsValidGridPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridManager.gridWidth &&
               position.y >= 0 && position.y < gridManager.gridHeight;
    }

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